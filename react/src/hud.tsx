import { useGlobals, useReactiveValue } from '@reactunity/renderer';

import './index.scss';

export default function Hud(): React.ReactNode {
    const globals = useGlobals();
    const timerText: string = useReactiveValue(globals.timerText);
    const money: number = useReactiveValue(globals.money);
    const distance: number = useReactiveValue(globals.distance);
    const totalDistance: number = useReactiveValue(globals.totalDistance);
    const distanceKm = (totalDistance - distance) / 1000;

    return <view className="game-hud">
        <view className="top-bar">
            <div className="left">
                <h3>SCORE</h3>
                <h2>{`$${money}`}</h2>
            </div>
            <div className="center">
                <h3>TIME UNTIL SUNSET</h3>
                <h1>{timerText}</h1>
            </div>
            <div className="right">
                <h3>TO DESTINATION</h3>
                <h2>{`${distanceKm.toFixed(1)} KM`}</h2>
            </div>
        </view>
        <view className="content">
        </view>
    </view>;
}
