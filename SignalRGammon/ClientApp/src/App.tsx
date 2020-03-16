import React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import './App.css';
import { GameSelector } from './components/GameSelector';
import { BackgammonComponent } from './components/backgammon/BackgammonComponent';
import { ChatComponent } from './components/chat/ChatComponent';

function App() {
  return (
    <Layout>
      <Route exact path='/' component={GameSelector} />
      <Route path='/backgammon' component={BackgammonComponent} />
      <Route path='/chat' component={ChatComponent} />
    </Layout>
  );
}

export default App;
