import './index.scss';

export default function LevelStart(): React.ReactNode {
    return (
        <view className="level-start">
            <view className="content">
                <h1 className="title">LEVEL START</h1>
                <button className="start-button">Start</button>
            </view>
        </view>
    );
}