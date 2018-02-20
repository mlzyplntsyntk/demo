import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export class Home extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
            <h1>3rd party consumer for Demo project</h1>
            <p>For demonstration purposes this project has 4 functions</p>
            <ul>
                <li>1. List of Latest Articles with the links to each article</li>
                <li>2. Read Article Page (will require authentication)</li>
                <li>3. Login Page</li>
                <li>4. Logout</li>
            </ul>
        </div>;
    }
}
