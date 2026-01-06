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
