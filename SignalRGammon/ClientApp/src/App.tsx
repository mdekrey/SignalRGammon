import React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import './App.css';
import { GameSelector } from './components/GameSelector';
import { BackgammonComponent } from './components/backgammon/BackgammonComponent';
import { ChatComponent } from './components/chat/ChatComponent';
import { LicensesComponent } from './components/LicensesComponent';
import { GameConnectionScope } from './services/gameConnectionContext';

function App() {
  return (
    <GameConnectionScope>
    <Layout>
      <Route exact path='/' component={GameSelector} />
      <Route path='/backgammon' component={BackgammonComponent} />
      <Route path='/chat' component={ChatComponent} />
      <Route path='/licenses' component={LicensesComponent} />
    </Layout>
    </GameConnectionScope>
  );
}

export default App;
