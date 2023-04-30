import React, { useState } from 'react';
import { useGlobals, useReactiveValue } from '@reactunity/renderer';

import './index.scss';

type Message = {
    speaker: string;
    text: string;
};

let conversations = new Map<string, Message[]>();
conversations["level1"] = [
    { speaker: "Handler", text: "You're already on the road? Good." },
    { speaker: "Handler", text: "This package is of utmost importance. You have 3 minutes to get it to the president." },
    { speaker: "Agent", text: "3 Minutes?!" },
    { speaker: "Handler", text: "Exactly. So better go fast." },
];
conversations["level2"] = [
    { speaker: "Handler", text: "By the way, there appears to be a pandemic going around." },
    { speaker: "Handler", text: "Could you also distribute these medicine at the marked buildings? We'll give you a bonus for it." },
];

export default function Dialogue(): React.ReactNode {
    const globals = useGlobals();

    const conversationKey: string = useReactiveValue(globals.conversationKey);
    const [index, setIndex] = useState(0);

    const currentConversation = conversations[conversationKey];
    const currentMessage = currentConversation ? currentConversation[index] : null;

    const handleNext = () => {
        // Advance conversation or call DialogueFinished
        const hasNext = currentConversation && currentConversation.length > index + 1;
        if (hasNext) {
            setIndex(index + 1);
        } else {
            Interop.GetType('ReactUnityBridge').DialogueFinished()
        }
    }

    return <view className="black-bar">
        <view className="content">
            <h1 className="title">{currentMessage ? currentMessage.speaker : ""}</h1>
            <view className="gradient-rule"></view>
            <p className="message">{currentMessage ? currentMessage.text : ""}</p>
        </view>
        <button onClick={() => handleNext()}>
            CONTINUE
        </button>
    </view>;
}