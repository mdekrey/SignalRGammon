import React from 'react';

const lorc = { author: "Lorc", authorUrl: "http://lorcblog.blogspot.com/" };
const delapouite = { author: "Delapouite", authorUrl: "http://delapouite.com" };
const ccBy3 = { license: "CC BY 3.0", licenseUrl: "http://creativecommons.org/licenses/by/3.0/" };
const gameIcons = { source: "game-icons.net", sourceUrl: "https://game-icons.net/" };

const licenses = [
    { title: "Backgammon icon", ...delapouite, ...ccBy3, ...gameIcons },
    { title: "Empty chessboard icon", ...delapouite, ...ccBy3, ...gameIcons },
    { title: "Crowned heart icon", ...lorc, ...ccBy3, ...gameIcons },
]

export function LicensesComponent(_: { children?: never }) {
    return (
        <ul>
            {licenses.map(entry =>
                <li key={entry.title}>
                    {entry.title}{" "}by{" "}
                    <a href={entry.authorUrl} rel="author">{entry.author}</a>
                    {" "}under{" "}
                    <a href={entry.licenseUrl} rel="license">{entry.license}</a>
                    {" "}via{" "}
                    <a href={entry.sourceUrl}>{entry.source}</a>
                </li>
            )}
        </ul>
    );
}
