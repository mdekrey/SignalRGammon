import React from 'react';
import { useRouteMatch, Switch, Route, RouteChildrenProps } from 'react-router-dom';
import { PlayCheckersComponent } from './PlayCheckersComponent';
import { NewGameComponent } from './NewGameComponent';
import { CheckersScope, CheckersScopeProps } from './CheckersService';
import { Omit } from 'react-router';

export function CheckersComponent() {
    const { path } = useRouteMatch();

    return (
        <Switch>
            <Route exact path={path} component={NewGameComponent} />
            <Route exact path={`${path}/:gameId/:playerColor`}>
                {
                    ({ match }: RouteChildrenProps<Omit<CheckersScopeProps, "children">>) =>
                        <CheckersScope  {...match!.params}>
                            <PlayCheckersComponent />
                        </CheckersScope>
                }
            </Route>
        </Switch>
    )
}
