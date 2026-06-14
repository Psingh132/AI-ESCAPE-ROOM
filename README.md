# 🎮 Escape Room Generation Engine

An AI-powered Escape Room Generator that creates immersive, story-driven escape room experiences for both kids and adults. The application leverages AI to dynamically generate complete escape rooms consisting of narratives, puzzles, clues, and solutions based on audience type, theme, and difficulty level.

Built with:

- ⚡ ASP.NET Core Web API
- 🅰️ Angular
- 🤖 AI-Powered Puzzle Generation

---

## 📖 Overview

The Escape Room Generation Engine allows users to generate unique escape room adventures on demand. Users can choose:

- Target Audience (Adult or Kid)
- Theme
- Difficulty Level

The AI then generates a fully structured escape room consisting of:

- An immersive introduction
- Three interconnected puzzles
- Story progression between puzzles
- Verified puzzle solutions
- Escape/victory ending

---

## ✨ Features

### 🎯 Dynamic Escape Room Generation

Generate unique escape rooms based on:

- Audience
- Theme
- Difficulty

### 🎭 Multiple Themes

#### Adult Themes

- Chronos Paradox
- Sunken Citadel
- Deep Space Derelict

#### Kid Themes

- Enchanted Forest
- Safari Zoo
- Mystery Park

### 🧩 Multiple Puzzle Types

#### Adult Puzzle Types

- Caesar Cipher
- Crack The Code
- Multi-Stage Escape Chains
- Mathematical Patterns
- Word Jumbles

#### Kid Puzzle Types

- Arithmetic Sequences
- Word Anagrams
- Alpha Count Ciphers
- Counting Riddles
- Object-Based Riddles

### 🔒 Puzzle Validation

The system ensures:

- Every puzzle has exactly one solution
- Clues align with puzzle logic
- Story remains consistent
- Difficulty rules are respected
- Puzzle progression is coherent

---

## 🏗️ Architecture

```text
+------------------+
| Angular Frontend |
+--------+---------+
         |
         | HTTP Requests
         v
+-----------------------+
| ASP.NET Core Web API  |
+-----------+-----------+
            |
            | Prompt Generation
            v
+-----------------------+
| AI Model / LLM        |
+-----------+-----------+
            |
            | JSON Response
            v
+-----------------------+
| Escape Room Engine    |
+-----------------------+
```

---

## 🧠 How It Works

### 1. User Selects Configuration

The user provides:

```json
{
  "targetAudience": "adult",
  "theme": "Chronos Paradox",
  "difficulty": "medium"
}
```

### 2. Prompt Generation

The backend builds a structured prompt containing:

- Audience rules
- Theme rules
- Difficulty rules
- Puzzle constraints
- JSON schema requirements

### 3. AI Room Generation

The AI generates:

- Room title
- Introduction
- Three connected puzzles
- Hints
- Solutions
- Story progression

### 4. Response Validation

The generated content is validated to ensure:

- Valid JSON structure
- Puzzle consistency
- Correct solution mapping
- Theme compliance

---

## 📋 Escape Room JSON Structure

```json
{
  "roomName": "The Shattered Meridian",
  "introduction": "Story introduction...",
  "difficulty": "medium",
  "theme": "chronos_paradox",
  "puzzles": [
    {
      "sequenceNumber": 1,
      "puzzleTitle": "Puzzle Title",
      "description": "Puzzle description",
      "cluesProvided": [
        "Hint 1",
        "Hint 2"
      ],
      "expectedSolution": "ANSWER",
      "unlocksText": "Narrative progression"
    }
  ]
}
```

---

## 🛠️ Technology Stack

### Backend

- ASP.NET Core Web API
- C#
- RESTful APIs
- JSON Serialization

### Frontend

- Angular
- TypeScript
- Angular HTTP Client
- Responsive UI

### AI Integration

- Large Language Models (LLMs)
- Prompt Engineering
- Structured JSON Output
- Knowledge Base Driven Generation

---
