import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link, NavLink } from 'react-router-dom';
import * as $ from 'jquery';

interface LoginState {
    EMailAddress: string,
    Password: string,
    user: any;
}

export class Login extends React.Component<RouteComponentProps<{}>, LoginState> {
    constructor() {
        super();
        let token = localStorage.getItem("token"),
            item = null;
        if (token) {
            try {
                item = JSON.parse(token);
            } catch (e) { }
        }
        this.state = { EMailAddress: "edward.clarkson@sunexpress.com", Password:"9zd33dfa", user: item }
    }
    submit() {
        $.ajax({
            url: "http://localhost:5000/data/login",
            method: "POST",
            data: {
                username: this.state.EMailAddress,
                password: this.state.Password
            }
        }).done(result => {
            if (typeof(result) === "undefined") {
                alert("No way");
                return;
            }
            localStorage.setItem("token", JSON.stringify(result));
            this.setState({
                user: result
            })
        })
    }
    logout() {
        localStorage.removeItem("token");
        this.setState({user: null})
    }
    valueChange(e: any) {
        var data:any = {};
        data[e.target.id] = e.target.value;
        this.setState(data);
    }
    public render() {
        return (
            this.state.user == null ?
            <div className="panel panel-default">
                    <div className="panel-body">
                        <div className="alert alert-info">You need to obtain a username and password from Users table, it does not matter if the user is Manager or not.</div>
                    <div className="form-group">
                        <label>EmailAdress</label>
                        <input id="EMailAddress" className="form-control" value={this.state.EMailAddress} onChange={this.valueChange.bind(this)} />
                    </div>
                    <div className="form-group">
                        <label>Password</label>
                        <input id="Password" className="form-control" value={this.state.Password} onChange={this.valueChange.bind(this)} />
                    </div>
                    <button className="btn btn-primary" onClick={this.submit.bind(this)}>Log In</button>
                </div>
            </div>  
                :
            <div>
                    Logged in as {this.state.user.firstName} {this.state.user.lastName}
                    <a onClick={this.logout.bind(this)}>Logout</a>
            </div>
        )
    }
}
