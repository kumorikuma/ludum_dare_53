import React, { useState } from 'react';
import { useGlobals, useReactiveValue } from '@reactunity/renderer';

import './index.scss';

type Message = {
    speaker: string;
    text: string;
};

let conversations = new Map<string, Message[]>();
conversations["game_intro"] = [
    { speaker: "Handler", text: "You're already on the road? Good. This package is of utmost importance. \nYou have 3 minutes to get it to the president." },
    { speaker: "Agent", text: "3 Minutes?!" },
    { speaker: "Handler", text: "Yeah, so be quick. And don't worry if the car gets a little banged up. \nWe'll just deduct the repair fees from your pay." },
];
conversations["delivery_intro"] = [
    { speaker: "Handler", text: "By the way, there appears to be a pandemic going around." },
    { speaker: "Handler", text: "Could you also distribute these medicine at the marked buildings? We'll give you a bonus for it." },
];
conversations["ending_good"] = [
    { speaker: "Agent", text: "Mr. President? Express delivery for you." },
    { speaker: "President", text: "Ah! My bingsu. I was waiting for this." },
    { speaker: "President", text: "And it hasn't melted yet. Well done. We'll give you a promotion." },
];
conversations["ending_bad"] = [
    { speaker: "Agent", text: "Mr. President? Express delivery for you." },
    { speaker: "President", text: "Ah! My bingsu. I was waiting for this." },
    { speaker: "President", text: "Wait. What is this? It's all MELTED! Have this guy fired!" },
];

export default function Dialogue(): React.ReactNode {
    const globals = useGlobals();

    const conversationKey: string = useReactiveValue(globals.conversationKey);
    const [index, setIndex] = useState(0);

    const currentConversation = conversations[conversationKey];
    const currentMessage = currentConversation ? currentConversation[index] : null;
    const hasNext = currentConversation && currentConversation.length > index + 1;

    const handleNext = () => {
        // Advance conversation or call DialogueFinished
        if (hasNext) {
            setIndex(index + 1);
        } else {
            Interop.GetType('ReactUnityBridge').DialogueFinished()
        }
    }

    return <view className="dialogue-bar">
        <view className="content">
            <h1 className="title">{currentMessage ? currentMessage.speaker : ""}</h1>
            <view className="gradient-rule"></view>
            <p className="message">{currentMessage ? currentMessage.text : ""}</p>
            <view className="gradient-rule"></view>
        </view>
        <button onClick={() => handleNext()}>
            {hasNext ? "CONTINUE" : "START"}
        </button>
    </view>;
}