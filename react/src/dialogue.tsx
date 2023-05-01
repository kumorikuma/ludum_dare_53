import React, { useState } from 'react';
import { useGlobals, useReactiveValue } from '@reactunity/renderer';
import { useEffect } from 'react';

import './index.scss';

type Message = {
    speaker: string;
    text: string;
};

let conversations = new Map<string, Message[]>();
conversations["game_intro"] = [
    { speaker: "Handler", text: "You got the package? Good." },
    { speaker: "Handler", text: "You must get it to the Director before sundown, which is... in 3 minutes. You got that?" },
    { speaker: "Agent", text: "3 Minutes?!" },
    { speaker: "Handler", text: "Yeah, so step on the gas. And careful not to get the company car damaged. \nWe'll be deducting the repair fees from your pay." },
    { speaker: "Handler", text: "Use [W] [A] [S] [D] keys to move." }
];
conversations["boost_intro"] = [
    { speaker: "Handler", text: "Buddy, could you be going any slower?! At this rate you're never gonna make it!" },
    { speaker: "Handler", text: "Look, I made some calls and got them to unlock your car's boost ability. \nIsn't over-the-air updates great?" },
    { speaker: "", text: "Your car can now go faster. Use [W] to accelerate." },
];
conversations["delivery_intro"] = [
    { speaker: "Handler", text: "Oh, I forgot to mention. The car upgrade costs money which we'll be deducting from your pay." },
    { speaker: "Agent", text: "What?!" },
    { speaker: "Handler", text: "Hey, don't worry, I found another gig for you." },
    { speaker: "Handler", text: "You know how there's a deadly disease spreading around the city, and our corporation has a monopoly on the cure? \nWe finally got a deal with the hospitals." },
    { speaker: "Handler", text: "Drop off the cures in your car at hospitals along the way, and we'll give you a bonus." },
    { speaker: "", text: "Press [E] to drop off cures." },
];
conversations["jump_into"] = [
    { speaker: "Handler", text: "Looks like there's a lot of traffic ahead." },
    { speaker: "Handler", text: "I got them to unlock the vertical mobility system on your car. That should help you out." },
    { speaker: "", text: "Press [Space] to jump." },
];
conversations["unlimited_intro"] = [
    { speaker: "Handler", text: "You still got a long way to go man." },
    { speaker: "Handler", text: "Here, my final gift for you. I got them to remove all speed limits on your car. You'd better make it with this. Our careers depend on it." },
    { speaker: "", text: "Your car no longer has a speed limit." },
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
    const continueValue = useReactiveValue(globals.continue);

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
    
    useEffect(() => {
        if (continueValue > 0) {
            handleNext();
            Interop.GetType('ReactUnityBridge').ResetContinue();
        }
    }, [continueValue, handleNext]);

    return <view className="dialogue-bar">
        <view className="content">
            <h1 className="title">{currentMessage ? currentMessage.speaker : ""}</h1>
            <view className="gradient-rule"></view>
            <p className="message">{currentMessage ? currentMessage.text : ""}</p>
        </view>
        <button onClick={() => handleNext()}>
            {hasNext ? "CONTINUE" : "START"}
        </button>
    </view>;
}