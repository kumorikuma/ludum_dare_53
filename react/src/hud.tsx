import { useGlobals, useReactiveValue } from '@reactunity/renderer';

import './index.scss';

export default function Hud(): React.ReactNode {
    const globals = useGlobals();
    const timerText = useReactiveValue(globals.timerText);
    const damagesText = useReactiveValue(globals.damagesText);

    return <view className="game-hud">
        <view className="top-bar">
            <h1 className="title">{timerText}</h1>
            <view className="spacer"></view>
            <h1 className="title">{damagesText}</h1>
        </view>
        <view className="content">
        </view>
    </view>;
}
