import React, { useMemo, useRef, useEffect, useReducer, useState } from 'react';

import { HubConnectionBuilder } from '@microsoft/signalr';

export function ChatComponent() {
    const [userName, setUserName] = useState('');
    const [message, setMessage] = useState('');
    const [sendEnabled, setSendEnabled] = useState(false);
    const [messages, dispatch] = useReducer((state: string[], newMessage: string) => [...state, newMessage], []);
    const sendButton = useRef<HTMLInputElement>(null);
    const connection = useMemo(() => new HubConnectionBuilder().withUrl("/chatHub").build(), []);

    useEffect(() => {

        connection.on("ReceiveMessage", function (user, message) {
            dispatch(user + " says " + message);
        });

        connection.start().then(function () {
            setSendEnabled(true);
        }).catch(function (err) {
            return console.error(err.toString());
        });

    }, [sendButton, connection]);

    return (
        <>
            <div className="container">
                <div className="row">&nbsp;</div>
                <div className="row">
                    <div className="col-2">User</div>
                    <div className="col-4"><input type="text" value={userName} onChange={(e) => setUserName(e.currentTarget.value)} /></div>
                </div>
                <div className="row">
                    <div className="col-2">Message</div>
                    <div className="col-4"><input type="text"  value={message} onChange={(e) => setMessage(e.currentTarget.value)} /></div>
                </div>
                <div className="row">&nbsp;</div>
                <div className="row">
                    <div className="col-6">
                        <input type="button" id="sendButton" value="Send Message" ref={sendButton} disabled={!sendEnabled} onClick={onSend} />
                    </div>
                </div>
            </div>
            <div className="row">
                <div className="col-12">
                    <hr />
                </div>
            </div>
            <div className="row">
                <div className="col-6">
                    <ul>
                        {messages.map((msg, idx) => <li key={idx}>{msg}</li>)}
                    </ul>
                </div>
            </div>
        </>
    );

    function onSend() {
        connection.invoke("SendMessage", userName, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
}