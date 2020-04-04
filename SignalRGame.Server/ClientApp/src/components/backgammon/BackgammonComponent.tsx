import React from 'react';
import { useRouteMatch, Switch, Route, RouteChildrenProps } from 'react-router-dom';
import { PlayBackgammonComponent } from './PlayBackgammonComponent';
import { NewGameComponent } from './NewGameComponent';
import { BackgammonScope, BackgammonScopeProps } from './BackgammonService';
import { Omit } from 'react-router';

export function BackgammonComponent() {
    const { path } = useRouteMatch();

    return (
        <Switch>
            <Route exact path={path} component={NewGameComponent} />
            <Route exact path={`${path}/:gameId/:playerColor`}>
                {
                    ({ match }: RouteChildrenProps<Omit<BackgammonScopeProps, "children">>) =>
                        <BackgammonScope  {...match!.params}>
                            <PlayBackgammonComponent />
                        </BackgammonScope>
                }
            </Route>
        </Switch>
    )
}
