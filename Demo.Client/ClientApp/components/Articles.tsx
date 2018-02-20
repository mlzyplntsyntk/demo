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

interface ArticlesState {
    articles: Article[];
    loading: boolean;
}

export class Articles extends React.Component<RouteComponentProps<{}>, ArticlesState> {
    constructor() {
        super();
        this.state = { articles: [], loading: true };

        $.ajax({
            url: 'http://localhost:5000/data/articles?pageNumber=1&pageSize=20&orderby=latest&withPaging=true'
        }).done(response => {
            this.setState({ articles: response.articles as Article[], loading: false });
        })
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Articles.renderArticles(this.state.articles);

        return <div>
            <h1>Articles</h1>
                <p> .</p>
            { contents }
        </div>;
    }

    private static renderArticles(articles: Article[]) {
        return <div>
            {
                articles.map(item =>
                    <tr key={ item.id }>
                        <td><NavLink to={'/read/'+item.id} activeClassName='active'>{item.title} </NavLink></td>
                    <td> {item.author} </td>
                    <td> {item.totalLikes} </td>
                    <td> {item.totalReads} </td>
                    <td> {item.creationTime} </td>
                    </tr>
                )
            }
        </div>;
    }
}
