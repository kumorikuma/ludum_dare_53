import { useGlobals, useReactiveValue } from '@reactunity/renderer';
import { render } from '@reactunity/renderer';
import './index.scss';

import { MemoryRouter, Route, Routes, useNavigate } from 'react-router';
import { useEffect } from 'react';

import Title from './title';
import Pause from './pause';
import LevelStart from './level_start';
import LevelEnd from './level_end';
import Hud from './hud';
import Dialogue from './dialogue';

export default function App() {
  const globals = useGlobals();
  const route = useReactiveValue(globals.route);

  const navigate = useNavigate();

  useEffect(() => {
    console.log(`Navigate to route: ${route}`);
    navigate(route);
  }, [route, navigate])

  return (
    <Routes>
      <Route path="/" element={<view />} />
      <Route path="/title" element={<Title />} />
      <Route path="/pause" element={<Pause />} />
      <Route path="/level_start" element={<LevelStart />} />
      <Route path="/level_end" element={<LevelEnd />} />
      <Route path="/hud" element={<Hud />} />
      <Route path="/dialogue" element={<Dialogue />} />
    </Routes>
  );
}

render(
  <MemoryRouter>
    <App />
  </MemoryRouter>
);
