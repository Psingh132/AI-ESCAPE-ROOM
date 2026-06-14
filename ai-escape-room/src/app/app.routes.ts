import { Routes } from '@angular/router';
import { GameComponent } from './components/game-component/game-component';

export const routes: Routes = [
	{ path: '', pathMatch: 'full', redirectTo: 'game' },
	{ path: 'game', component: GameComponent },
	// future routes: settings, credits, leaderboard
];
