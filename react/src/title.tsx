import './index.scss';

export default function TitleScreen(): React.ReactNode {

    return <view className="title-screen">
        <h1 className="title">GAME TITLE</h1>
        <button onClick={() =>
            Interop.GetType('ReactUnityBridge').StartGameClicked()
        }>
            START
        </button>
    </view>;
}
