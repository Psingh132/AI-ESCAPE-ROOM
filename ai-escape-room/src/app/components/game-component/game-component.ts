import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FoundryIqService, EscapeRoomResponse, PuzzleDto } from '../../services/foundry-iq.service';

@Component({
  selector: 'app-game-component',
  imports: [CommonModule],
  templateUrl: './game-component.html',
  styleUrls: ['./game-component.css'],
})
export class GameComponent {
  audience: 'Adult' | 'Child' = 'Adult';

  adultThemes = [
    { id: 'Chronos Paradox', displayName: 'Chronos Paradox' },
    { id: 'Sunken Citadel', displayName: 'Sunken Citadel' },
    { id: 'Deep Space Derelict', displayName: 'Deep Space Derelict' },
  ];

  childThemes = [
    { id: 'Enchanted Forest', displayName: 'Enchanted Forest' },
    { id: 'Safari Zoo', displayName: 'Safari Zoo' },
    { id: 'Mystery Park', displayName: 'Mystery Park' },
  ];

  get themeOptions() {
    return this.audience === 'Adult' ? this.adultThemes : this.childThemes;
  }

  theme = this.themeOptions[0].id;
  difficulty: 'Easy' | 'Medium' | 'Hard' = 'Medium';
  loading = false;
  gameStarted = false;
  gameCompleted = false;
  gameFailed = false;
  currentRoom: EscapeRoomResponse | null = null;
  currentPuzzleIndex = 0;
  currentPuzzle: PuzzleDto | null = null;
  messages: string[] = [];
  playerAnswer = '';
  feedback = '';
  error = '';
  attemptCount = 0;
  maxAttempts = 3;

  constructor(private foundry: FoundryIqService, private cdr: ChangeDetectorRef) {}

  onAudienceChange(value: 'Adult' | 'Child') {
    this.audience = value;
    const options = this.themeOptions;
    if (!options.some((option) => option.id === this.theme)) {
      this.theme = options[0].id;
    }
  }

  async startGame() {
    this.messages = [];
    this.feedback = '';
    this.playerAnswer = '';
    this.error = '';
    this.loading = true;
    this.gameStarted = true;
    this.gameCompleted = false;
    this.gameFailed = false;
    this.currentRoom = null;
    this.currentPuzzle = null;
    this.currentPuzzleIndex = 0;

    try {
      const response = await this.foundry.startGame(this.theme, this.difficulty, this.audience);
      this.initializeRoom(response);
    } catch (err) {
      this.error = 'Could not connect to the AI game server. Please try again.';
      this.gameStarted = false;
    } finally {
      this.loading = false;
    }
  }

  private initializeRoom(response: EscapeRoomResponse) {
    console.log('Response received:', response);
    this.currentRoom = response;
    this.messages.push(`Room: ${response.roomName}`);
    this.messages.push(response.introduction);
    this.currentPuzzleIndex = 0;
    this.loading = false;
    this.setCurrentPuzzle();
    this.cdr.detectChanges(); // Force change detection
    console.log('After setCurrentPuzzle - currentPuzzle:', this.currentPuzzle);
    console.log('gameStarted:', this.gameStarted);
  }

  private setCurrentPuzzle() {
    if (!this.currentRoom) {
      this.error = 'No room data available.';
      this.currentPuzzle = null;
      console.error(this.error);
      return;
    }
    
    if (!this.currentRoom.puzzles || !this.currentRoom.puzzles.length) {
      this.error = `No puzzles were returned from the server. Puzzles array: ${JSON.stringify(this.currentRoom.puzzles)}`;
      this.currentPuzzle = null;
      console.error(this.error);
      return;
    }

    this.currentPuzzle = this.currentRoom.puzzles[this.currentPuzzleIndex];
    this.playerAnswer = '';
    this.feedback = '';
    this.attemptCount = 0; // Reset attempts for new puzzle
    console.log('Setting current puzzle:', this.currentPuzzle);
    if (this.currentPuzzle && this.currentPuzzle.sequenceNumber !== this.currentPuzzleIndex + 1) {
      this.currentPuzzle.sequenceNumber = this.currentPuzzleIndex + 1;
    }
  }

  submitAnswer() {
    if (!this.currentPuzzle) return;

    const normalized = this.playerAnswer.trim().toLowerCase();
    const correct = this.currentPuzzle.expectedSolution.trim().toLowerCase();
    if (normalized === correct) {
      this.feedback = 'Correct. The lock opens.';
      this.messages.push(this.feedback);
      this.messages.push(this.currentPuzzle.unlocksText);
      this.playerAnswer = '';
      this.advancePuzzle();
    } else {
      this.attemptCount++;
      if (this.attemptCount >= this.maxAttempts) {
        const failureMessages = [
          '💀 System lockdown initiated. You failed to escape.',
          '⚠️ The alarm sounds... Your time is up. Mission failed.',
          '🚨 Containment protocols activated. You are trapped.',
          '❌ Critical failure. The chamber seals forever.',
          '🌪️ The pressure builds... You failed to survive.'
        ];
        const randomFailure = failureMessages[Math.floor(Math.random() * failureMessages.length)];
        this.feedback = randomFailure;
        this.messages.push(this.feedback);
        this.currentPuzzle = null;
        this.gameStarted = false;
        this.gameFailed = true;
        this.gameCompleted = false;
        this.cdr.detectChanges();
      } else {
        const remaining = this.maxAttempts - this.attemptCount;
        this.feedback = `Incorrect. ${remaining} attempt${remaining === 1 ? '' : 's'} remaining.`;
      }
    }
  }

  askHint() {
    if (!this.currentPuzzle || !this.currentPuzzle.cluesProvided?.length) return;
    const usedHints = this.messages.filter((msg) => msg.startsWith('Clue: ')).length;
    if (usedHints >= this.currentPuzzle.cluesProvided.length) {
      this.messages.push('No more hints are available for this puzzle.');
      return;
    }

    const hint = this.currentPuzzle.cluesProvided[usedHints];
    this.messages.push(`Clue: ${hint}`);
  }

  private advancePuzzle() {
    if (!this.currentRoom) return;

    this.currentPuzzleIndex += 1;
    if (this.currentPuzzleIndex < this.currentRoom.puzzles.length) {
      this.setCurrentPuzzle();
    } else {
      this.messages.push('All puzzles complete. You have escaped!');
      this.currentPuzzle = null;
      this.gameStarted = false;
      this.gameCompleted = true;
      this.gameFailed = false;
    }
  }

  resetGame() {
    this.gameStarted = false;
    this.loading = false;
    this.gameCompleted = false;
    this.gameFailed = false;
    this.currentRoom = null;
    this.currentPuzzle = null;
    this.currentPuzzleIndex = 0;
    this.messages = [];
    this.playerAnswer = '';
    this.feedback = '';
    this.error = '';
    this.attemptCount = 0;
  }
}
