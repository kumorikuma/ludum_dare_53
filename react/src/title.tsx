import './index.scss';

import { useGlobals, useReactiveValue } from '@reactunity/renderer';
import { useEffect } from 'react';

export default function TitleScreen(): React.ReactNode {
    const globals = useGlobals();
    const continueValue = useReactiveValue(globals.continue);
    useEffect(() => {
        if (continueValue > 0) {
            Interop.GetType('ReactUnityBridge').ResetContinue();
            Interop.GetType('ReactUnityBridge').StartGameClicked();
        }
    }, [continueValue]);

    return <view className="title-screen">
        <h1>【Ｎｉｇｈｔ　Ｄｅｌｉｖｅｒｙ】</h1>
        <button onClick={() =>
            Interop.GetType('ReactUnityBridge').StartGameClicked()
        }>
            START
        </button>
    </view>;
}
