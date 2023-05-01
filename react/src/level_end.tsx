import './index.scss';
import { useGlobals, useReactiveValue } from '@reactunity/renderer';

// Pad number with leading zeros
function pad(num, size) {
    num = num.toString();
    while (num.length < size) num = "0" + num;
    return num;
}

// Format to mm:ss
function formatTime(seconds: number): string {
    const negative = seconds < 0 ? "-" : "";
    const absSeconds = seconds < 0 ? -seconds : seconds;
    const mins = Math.floor(absSeconds / 60);
    const secs = Math.floor(absSeconds - (mins * 60));
    return `${negative}${mins}:${pad(secs, 2)}`;
}

export default function LevelEnd(): React.ReactNode {
    const globals = useGlobals();

    const scoreLevel: number = useReactiveValue(globals.scoreLevel);
    const scoreTime: number = useReactiveValue(globals.scoreTime);
    const scoreTimeLimit: number = useReactiveValue(globals.scoreTimeLimit);
    const scoreDamages: number = useReactiveValue(globals.scoreDamages);
    const scoreDeliveries: number = useReactiveValue(globals.scoreDeliveries);
    const scoreDeliveriesGoal: number = useReactiveValue(globals.scoreDeliveriesGoal);
    const scoreEarnings: number = useReactiveValue(globals.scoreEarnings);

    const handleNext = () => {
        Interop.GetType('ReactUnityBridge').RestartGameClicked();
    }

    return (
        <view className="level-end">
            <h1 className="title">{`GAME OVER`}</h1>
            <h2 className="stat">{`TIME REMAINING: ${formatTime(scoreTime)}`}</h2>
            <h2 className="stat">{`CURES DELIVERED: ${scoreDeliveries}`}</h2>
            <h2 className="stat">{`COLLISIONS: ${scoreDamages}`}</h2>
            {/* <h2 className="stat">{`SCORE: ${scoreEarnings}`}</h2> */}
            <button onClick={() => handleNext()}>
                RESTART
            </button>
        </view>
    );
}