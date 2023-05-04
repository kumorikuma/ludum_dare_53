import './index.scss';

import { useGlobals, useReactiveValue } from '@reactunity/renderer';
import { useEffect } from 'react';

export default function TitleScreen(): React.ReactNode {
    const globals = useGlobals();
    const continueValue: number = useReactiveValue(globals.continue);
    useEffect(() => {
        if (continueValue > 0) {
            Interop.GetType('ReactUnityBridge').ResetContinue();
            Interop.GetType('ReactUnityBridge').StartGameClicked();
        }
    }, [continueValue]);

    return <view className="title-screen">
        <h1>　　　</h1>
        {/*<h1>【Ｓｕｎｓｅｔ　Ｄｅｌｉｖｅｒｙ】</h1>*/}
        <button onClick={() =>
            Interop.GetType('ReactUnityBridge').StartGameClicked()
        }>
            START
        </button>
    </view>;
}
