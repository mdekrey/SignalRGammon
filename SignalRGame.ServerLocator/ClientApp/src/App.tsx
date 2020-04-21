import React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import './App.css';
import { GameSelector } from './components/GameSelector';
import { NestedGameComponent } from './components/NestedGameComponent';
import { LicensesComponent } from './components/LicensesComponent';

function App() {
  return (
      <Layout>
        <Route exact path='/' component={GameSelector} />
        <Route path='/:gameType/:gameId' component={NestedGameComponent} />
        <Route path='/licenses' component={LicensesComponent} />
      </Layout>
  );
}

export default App;
