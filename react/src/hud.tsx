import { useGlobals, useReactiveValue } from '@reactunity/renderer';

import './index.scss';

export default function Hud(): React.ReactNode {
    const globals = useGlobals();
    const timerText = useReactiveValue(globals.timerText);
    const damagesText = useReactiveValue(globals.damagesText);

    return <view className="game-hud">
        <view className="top-bar">
            <div className="left">
                <h2>$999</h2>
            </div>
            <div className="center">
                <h1>{timerText}</h1>
            </div>
            <div className="right">
                <h2>{damagesText}</h2>
            </div>
        </view>
        <view className="content">
        </view>
    </view>;
}
