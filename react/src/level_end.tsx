import './index.scss';

export default function LevelEnd(): React.ReactNode {
    return (
        <view className="level-end">
            <view className="content">
                <h1 className="title">LEVEL END</h1>
                <button className="next-level-button">Next Level</button>
            </view>
        </view>
    );
}