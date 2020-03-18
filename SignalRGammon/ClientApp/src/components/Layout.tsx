import React from 'react';
import './Layout.css';


export function Layout(props: { children: React.ReactNode }) {
    return (
        <div className="Layout">
            <div className="Layout-Child">
                {props.children}
            </div>
        </div>
    );
}
