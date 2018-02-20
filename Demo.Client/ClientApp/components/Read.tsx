import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Link, NavLink } from 'react-router-dom';
import * as $ from 'jquery';

interface Article {
    id: number;
    title: string;
    author: string;
    totalLikes: number;
    totalReads: number;
    creationTime: Date;
}

interface ArticleState {
    article: any;
    loading: boolean;
}

export class Read extends React.Component<RouteComponentProps<{}>, ArticleState> {
    constructor() {
        super();
        this.state = {article: {}, loading: true };

        let token = localStorage.getItem("token"),
            item = null;
        if (token) {
            try {
                item = JSON.parse(token);
            } catch (e) { }
        }
        console.log(this);
        $.ajax({
            url: 'http://localhost:5000/data/5',
            headers: {
                'Authorization': (item != null ? item.userToken : "no")
            }
        }).done(response => {
            this.setState({ article: response.article as Article, loading: false });
        })
    }

    public render() {

        return !this.state.article.title ? <div>Not Authorized please login</div>
            :
            <div>
                <h1>{this.state.article.title}</h1>
                <p>{this.state.article.content}</p>
            </div>;
    }
}
