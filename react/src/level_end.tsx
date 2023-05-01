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
    var mins = Math.floor(seconds / 60);
    var secs = Math.floor(seconds - (mins * 60));
    return `${mins}:${pad(secs, 2)}`;
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
        Interop.GetType('ReactUnityBridge').NextLevelClicked()
    }

    return (
        <view className="level-end">
            <h1 className="title">{`CHECKPOINT ${scoreLevel} REACHED`}</h1>
            <h2 className="stat">{`TIME: ${formatTime(scoreTime)} / ${formatTime(scoreTimeLimit)}`}</h2>
            <h2 className="stat">{`ANTIDOTES DELIVERED: ${scoreDeliveries} / ${scoreDeliveriesGoal}`}</h2>
            <h2 className="stat">{`DAMAGES: $${scoreDamages}`}</h2>
            <h2 className="stat">{`EARNINGS: $${scoreEarnings}`}</h2>
            <button onClick={() => handleNext()}>
                NEXT
            </button>
        </view>
    );
}