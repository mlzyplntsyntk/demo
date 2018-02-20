import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Articles } from './components/Articles';
import { Read } from './components/Read';
import { Login } from './components/Login';
import * as $ from 'jquery';


export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/articles' component={Articles} />
    <Route path='/read/:id' component={Read} />
    <Route path='/login' component={Login} />
</Layout>;
