import logo from './logo.svg';
import './App.css';
import Weather from './components/weather';


function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Hello World
        </p>
        
        <Weather />

      </header>
    </div>
  );
}

export default App;
