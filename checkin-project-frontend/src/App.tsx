// src/App.tsx
import React from "react";
import { Header } from "./components/Layout/Header";
import { AppRouter } from "./routes/AppRouter";

const App: React.FC = () => {
  return (
    <div className="app-root">
      <Header />
      <main className="app-main">
        <AppRouter />
      </main>
    </div>
  );
};

export default App;
