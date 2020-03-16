import React from 'react';


export function Layout(props: { children: React.ReactNode }) {
    return (
        <div>
            {props.children}
        </div>
    );
}
