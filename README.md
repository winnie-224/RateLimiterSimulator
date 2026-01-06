# API Gateway Rate Limiter Simulator (WPF)

A WPF-based desktop simulator that implements and visualizes API Gateway rate limiting algorithms, allowing real-time comparison of Token Bucket and Sliding Window strategies.

---

## Project Overview

This project simulates how an **API Gateway** enforces rate limiting policies before requests reach backend services.  
It is designed as a **system-design learning tool**, not a toy app.

Key goals:
- Understand rate limiting behavior under real traffic
- Compare algorithm trade-offs
- Visualize internal decisions and metrics

---

## System Design Concepts Covered

- API Gateway pattern
- Rate limiting as a cross-cutting concern
- Thread-safe in-memory counters
- Precision vs performance trade-offs
- Observability in system design

---

## Implemented Rate Limiting Algorithms

### Token Bucket
- Allows short bursts
- Enforces average rate
- Lazy token refill (no background threads)
- Best for user-facing APIs

### Sliding Window
- Strict enforcement
- No burst allowance
- Higher precision
- Suitable for sensitive APIs (login, payments)

Both algorithms implement a common interface:

```csharp
IRateLimiter
```
---
## Screenshots

<img width=40% height=relative alt="Screenshot 2026-01-06 151008" src="https://github.com/user-attachments/assets/71517b1b-a10a-4df1-b24b-869de4e102ec" />
<img width=40% alt="Screenshot 2026-01-06 151021" src="https://github.com/user-attachments/assets/23e8928d-b2c5-4bf0-9726-3dfe7629dea9" />
<img width=40% height=40% alt="Screenshot 2026-01-06 151034" src="https://github.com/user-attachments/assets/94625743-532a-4ddb-b32d-85e834be4a5c" />
