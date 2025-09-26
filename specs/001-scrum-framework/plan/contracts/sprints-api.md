# Sprints API Contract

**Base URL**: `/api/teams/{teamId}/sprints`  
**Authentication**: Required  
**Version**: v1

## Endpoints

### GET /api/teams/{teamId}/sprints
**Purpose**: Retrieve team's sprints  
**Authorization**: Team member  

**Query Parameters**:
- `status`: Filter by sprint status (optional)
- `limit`: Number of results (default 10, max 50)
- `offset`: Pagination offset (default 0)

**Request**:
```http
GET /api/teams/1/sprints?status=Active&limit=5
Accept: application/json
```

**Response** (200 OK):
```json
{
  "sprints": [
    {
      "id": 15,
      "name": "Sprint 15",
      "goal": "Complete user authentication and profile management",
      "startDate": "2025-01-20T00:00:00Z",
      "endDate": "2025-02-03T00:00:00Z",
      "status": "Active",
      "capacity": 40,
      "actualVelocity": null,
      "backlogItemCount": 8,
      "completedItemCount": 3,
      "impedimentCount": 1
    }
  ],
  "totalCount": 1,
  "hasNext": false
}
```

### GET /api/teams/{teamId}/sprints/{sprintId}
**Purpose**: Retrieve specific sprint with full details  
**Authorization**: Team member  

**Response** (200 OK):
```json
{
  "id": 15,
  "name": "Sprint 15",
  "goal": "Complete user authentication and profile management",
  "startDate": "2025-01-20T00:00:00Z",
  "endDate": "2025-02-03T00:00:00Z",
  "status": "Active",
  "capacity": 40,
  "actualVelocity": null,
  "notes": "First sprint with new team member",
  "backlogItems": [
    {
      "id": 101,
      "productBacklogItemId": 25,
      "title": "User login functionality",
      "status": "Done",
      "originalEstimate": 8,
      "remainingWork": 0,
      "taskCount": 3,
      "completedTaskCount": 3
    }
  ],
  "impediments": [
    {
      "id": 5,
      "title": "Database server downtime",
      "severity": "High",
      "status": "Open",
      "reportedDate": "2025-01-25T14:30:00Z"
    }
  ]
}
```

### POST /api/teams/{teamId}/sprints
**Purpose**: Create new sprint  
**Authorization**: Scrum Master or Product Owner  

**Request**:
```http
POST /api/teams/1/sprints
Content-Type: application/json

{
  "name": "Sprint 16",
  "goal": "Implement sprint management features",
  "startDate": "2025-02-03T00:00:00Z",
  "endDate": "2025-02-17T00:00:00Z",
  "capacity": 45
}
```

**Response** (201 Created):
```json
{
  "id": 16,
  "name": "Sprint 16",
  "goal": "Implement sprint management features",
  "startDate": "2025-02-03T00:00:00Z",
  "endDate": "2025-02-17T00:00:00Z",
  "status": "Planning",
  "capacity": 45,
  "actualVelocity": null,
  "notes": ""
}
```

**Business Rules**:
- Sprint dates must not overlap with existing active sprints
- Start date must be in the future
- Duration must match team's configured sprint length
- Only one active sprint per team at a time

### PUT /api/teams/{teamId}/sprints/{sprintId}
**Purpose**: Update sprint details  
**Authorization**: Scrum Master  

**Request**:
```http
PUT /api/teams/1/sprints/15
Content-Type: application/json

{
  "name": "Sprint 15 - Extended",
  "goal": "Complete user authentication and basic profile management",
  "capacity": 42,
  "notes": "Adjusted scope due to complexity"
}
```

### POST /api/teams/{teamId}/sprints/{sprintId}/start
**Purpose**: Start sprint (change status to Active)  
**Authorization**: Scrum Master  

**Request**:
```http
POST /api/teams/1/sprints/15/start
```

**Response** (200 OK):
```json
{
  "id": 15,
  "status": "Active",
  "startedAt": "2025-01-27T09:00:00Z"
}
```

### POST /api/teams/{teamId}/sprints/{sprintId}/complete
**Purpose**: Complete sprint  
**Authorization**: Scrum Master  

**Request**:
```http
POST /api/teams/1/sprints/15/complete
Content-Type: application/json

{
  "actualVelocity": 38,
  "notes": "Successfully completed 7 out of 8 stories"
}
```

## Sprint Backlog Management

### GET /api/teams/{teamId}/sprints/{sprintId}/backlog
**Purpose**: Get sprint backlog items  

**Response** (200 OK):
```json
{
  "items": [
    {
      "id": 101,
      "productBacklogItemId": 25,
      "title": "User login functionality",
      "description": "Implement secure user authentication",
      "status": "Done",
      "originalEstimate": 8,
      "remainingWork": 0,
      "tasks": [
        {
          "id": 301,
          "title": "Create login API endpoint",
          "status": "Done",
          "assignedTo": "John Doe",
          "remainingHours": 0
        }
      ]
    }
  ]
}
```

### POST /api/teams/{teamId}/sprints/{sprintId}/backlog
**Purpose**: Add product backlog item to sprint  
**Authorization**: Scrum Master or Product Owner  

**Request**:
```http
POST /api/teams/1/sprints/15/backlog
Content-Type: application/json

{
  "productBacklogItemId": 28,
  "originalEstimate": 13
}
```

### DELETE /api/teams/{teamId}/sprints/{sprintId}/backlog/{itemId}
**Purpose**: Remove item from sprint backlog  
**Authorization**: Scrum Master  

## Sprint Events

### GET /api/teams/{teamId}/sprints/{sprintId}/events
**Purpose**: Get sprint events (ceremonies)  

**Response** (200 OK):
```json
{
  "events": [
    {
      "id": 50,
      "type": "SprintPlanning",
      "title": "Sprint 15 Planning",
      "scheduledDate": "2025-01-20T09:00:00Z",
      "durationMinutes": 120,
      "status": "Completed",
      "participantCount": 5,
      "notes": "All stories estimated and committed"
    }
  ]
}
```

### POST /api/teams/{teamId}/sprints/{sprintId}/events
**Purpose**: Schedule sprint event  

**Request**:
```http
POST /api/teams/1/sprints/15/events
Content-Type: application/json

{
  "type": "DailyScrum",
  "scheduledDate": "2025-01-28T09:00:00Z",
  "durationMinutes": 15
}
```

## Sprint Metrics

### GET /api/teams/{teamId}/sprints/{sprintId}/burndown
**Purpose**: Get sprint burndown chart data  

**Response** (200 OK):
```json
{
  "sprintId": 15,
  "sprintDays": 14,
  "totalCapacity": 40,
  "burndownData": [
    {
      "date": "2025-01-20",
      "remainingWork": 40,
      "idealRemaining": 40
    },
    {
      "date": "2025-01-21",
      "remainingWork": 37,
      "idealRemaining": 37.1
    }
  ]
}
```

### GET /api/teams/{teamId}/sprints/{sprintId}/velocity
**Purpose**: Get sprint velocity calculation  

**Response** (200 OK):
```json
{
  "sprintId": 15,
  "plannedVelocity": 40,
  "actualVelocity": 38,
  "completedStoryPoints": 38,
  "totalStoryPoints": 45,
  "completionPercentage": 84.4
}
```

---
*Contract follows OpenAPI 3.0 specification and RESTful principles*