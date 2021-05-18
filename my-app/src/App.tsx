import React from 'react';
import './App.scss';
import NavBar from './components/NavBar/NavBar';
import SentimentTables from './components/SentimentTables/SentimentTables'

function App() {
  return (
    <div className="App">
      <NavBar />
      <SentimentTables />
    </div>
  );
}

export default App;
