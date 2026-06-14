import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

type Difficulty = 'Easy' | 'Medium' | 'Hard';

export interface PuzzleDto {
  sequenceNumber: number;
  puzzleTitle: string;
  description: string;
  cluesProvided: string[];
  expectedSolution: string;
  unlocksText: string;
}

export interface EscapeRoomResponse {
  roomName: string;
  introduction: string;
  difficulty: string;
  theme: string;
  puzzles: PuzzleDto[];
}

@Injectable({ providedIn: 'root' })
export class FoundryIqService {
  constructor(private http: HttpClient) {}

  startGame(theme: string, difficulty: Difficulty, audience: 'Adult' | 'Child') {
    return firstValueFrom(
      this.http.post<EscapeRoomResponse>('https://localhost:7075/Game/start', { theme, difficulty, audience })
    );
  }
}
