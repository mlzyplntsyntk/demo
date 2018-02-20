!(function (w, $) {
    $(document).ready(function () {
        app.onReady();
    });

    var app =  {
        eventQueue: [],
        onReady: function () {
            var $t = this;
            this.eventQueue.forEach(function (callback) {
                callback.call($t);
            });
            this.eventQueue = [];
        },
        ready: function (callback) {
            this.eventQueue.push(callback)
        },
        setVariable: function (key, value) {
            variables[key] = value;
        },
        getVariable: function (key) {
            return variables[key] || null;
        },
        httpGet: function (options) {
            options.method = "get";
            requestCall(options);
        },
        submitFormAsync: function (form, options) {
            form.on("submit", function (e) {
                e.preventDefault();

                if ($(this).validate().errorList.length > 0) return;

                var data = {};
                $(this).serializeArray().forEach(function (item) {
                    data[item.name] = item.value;
                });

                if (typeof CKEDITOR !== "undefined") {
                    for (var a in CKEDITOR.instances) {
                        data[a] = CKEDITOR.instances[a].getData();
                    }
                }

                var _options = {
                    url: $(this).attr("action"),
                    method: $(this).attr("method"),
                    data: data
                };

                if ($(this).attr("id") !== "authentication-form") {
                    for (var a in options) {
                        _options[a] = options[a];
                    }
                }

                requestCall(_options);
            });
        }
    };

    var selectComponent = {
        create: function (options) {
            var target = options.target,
                url = options.url,
                valueField = options.valueField,
                textField = options.textField,
                textFormatter = options.textFormatter;

            target.empty().append(newOption({ text: "Loading..." }));
            requestCall({
                url: url,
                method: "get",
                done: function (result) {
                    baseData[target] = result;
                    target.empty().append(newOption({ text: "Please Choose" }));
                    result.forEach(function (item) {
                        target.append(newOption({
                            val: item[valueField],
                            text: textField ? item[textField] : textFormatter.call(item)
                        }))
                    });
                }
            });

            if (options.onChange) {
                target.on("change", function () {
                    var selectedOption = null;
                    baseData[target].forEach(function (item) {
                        if (item[valueField].toString() === target.val()) {
                            selectedOption = item;
                        }
                    });
                    options.onChange.call(target, selectedOption);
                });
            }
        }
    }

    var modalComponent = {
        /**
         * Shows the modal window with the given options
         * options are as follows
         * title:string = Title of the Window
         * domContent:jQueryDomObject = a dom object wrapped with jQuery
         */
        domObject:null,
        show: function (options) {
            // creates an instance of the modal window
            // btw: this approach is not the best practice, the domObjects
            // should be created with native methods. (for performance issues)
            if (this.domObject == null) {

                this.domObject = $('<div class="modal fade single-modal-window" role="dialog">' +
                    '<div class="modal-dialog">' +
                        '<div class="modal-content">' +
                            '<div class="modal-header">' +
                                '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
                                '<h4 class="modal-title"></h4>' +
                            '</div>' +
                            '<div class="modal-body clearfix">' +

                            '</div>' +
                        '</div>' +
                    '</div>' +
                    '</div>');

                $("body").append(this.domObject);
            }
            this.clear();
            options.title && this.domObject.find("h4").html(options.title);
            options.domContent && this.domObject.find(".modal-body").append(options.domContent);
            this.domObject.modal("show");
        },
        clear: function () {
            this.domObject.find("h4, .modal-body").empty();
        },
        close: function () {
            this.clear();
            this.domObject.modal("hide");
        }
    }

    /**
     * PRIVATE FUNCTIONS and VARIABLES
     */
    var variables = {},
        baseData = {},
        newOption = function (args) {
            return $('<option>').val(args.val || "").text(args.text);
        },
        requestCall = function (options) {
            options.validateResult = options.validateResult || true;
            $.ajax({
                url: options.url,
                method: options.method,
                data: options.data
            }).done(function (result) {
                if (options.validateResult && !isAuthorized(result)) {
                    options.fail && options.fail.call(undefined, result);
                } else {
                    options.done && options.done.call(undefined, result);
                }
                options.always && options.always.call(undefined, result);
            });
        },
        // parse the entire html's body content to a jquery object
        // https://stackoverflow.com/questions/4429851/parse-complete-html-page-with-jquery
        bodyParser = function (bodyContent) {
            return $('<div id="body-mock">' + bodyContent.replace(/^[\s\S]*<body.*?>|<\/body>[\s\S]*$/ig, '') + '</div>')
        },
        // here i want to implement the ability to login without
        // navigating to login page, when an ajax request occurs.
        // sometimes it can be frustrating for editors who try to compose large
        // amounts of parapgraphs and hit the save button to see the Login page.
        // With .NetCore we can also achieve this implementation by using TempData
        // and it's built-in Keep function. 
        
        // this behaviour can be tested either from an article detail
        // page (Like Button is visible for all users) or from the 
        // Create Article page (need to click the End Session link)
        // to destroy the current session and hit save button 
        isAuthorized = function (result) {
            if (typeof result === "string") {
                var temp = bodyParser(result);
                // this method manages the event cycle mentioned below at STEP5.
                // simply checks for the temp document tree and decides what to 
                if (temp.find(".navbar-user-loggedin").length > 0) {
                    $(".navbar-user-profile").replaceWith(temp.find(".navbar-user-loggedin").clone());
                    hideLoginForm();
                    return true;
                }

                if (temp.find(".alert-danger").length > 0) {
                    modalComponent.domObject.find(".alert-danger").remove();
                    modalComponent.domObject.find(".modal-body").prepend(temp.find(".alert-danger").clone());
                    return false;
                }

                if (temp.find("#authentication-form").length > 0) {
                    showLoginForm(temp);
                    return false;
                }
            }
            return true;
        },
        hideLoginForm = function () {
            modalComponent.close();
        };
        // now the tricky part is here :
        // STEP1 : if the user needs to be authenticated we will need to first show 
        // the login page in a modal. 
        // STEP2 : Then we need to execute the scripts 
        // which are available in our temp object marked with data-async-exec
        // STEP3 : Because the scripts are designed to work after the document's
        // ready event fired, our previous execution won't change anything. Therefore
        // we need to trigger app.onReady function manually
        // STEP4 : for our client side scripts to work properly, we should force
        // validator to parse document again.
        // STEP5 : and finally, we need to override our loginForm's submit
        // event to support async request. when form submitted, the same cycle of 
        // events will be validated until user logs in succesfully.
        showLoginForm = function (temp) {
            var loginForm = temp.find("#authentication-form").clone();

            //STEP1
            modalComponent.show({
                domContent: loginForm,
                title: "Login Form"
            });
            //STEP2
            temp.find("script").each(function () {
                if ($(this).attr("data-async-exec")) {
                    eval($(this)[0].innerText);
                }
            });
            //STEP3
            app.onReady();
            //STEP4
            jQuery.validator.unobtrusive.parse(document);
            //STEP5
            app.submitFormAsync(loginForm);
        }

    app.selectComponent = selectComponent;

    w.app = app || {};
})(window, jQuery);