# Product Backlog API Contract

**Base URL**: `/api/teams/{teamId}/backlog`  
**Authentication**: Required  
**Version**: v1

## Endpoints

### GET /api/teams/{teamId}/backlog
**Purpose**: Retrieve team's product backlog  
**Authorization**: Team member  

**Query Parameters**:
- `status`: Filter by item status (optional)
- `type`: Filter by item type (optional)
- `limit`: Number of results (default 20, max 100)
- `offset`: Pagination offset (default 0)

**Request**:
```http
GET /api/teams/1/backlog?status=Ready&limit=10
Accept: application/json
```

**Response** (200 OK):
```json
{
  "backlog": {
    "id": 1,
    "teamId": 1,
    "lastRefinedDate": "2025-01-25T14:00:00Z"
  },
  "items": [
    {
      "id": 25,
      "title": "User login functionality",
      "description": "As a user, I want to log into the system so that I can access my personal dashboard",
      "acceptanceCriteria": "- User can enter email and password\n- System validates credentials\n- User is redirected to dashboard on success",
      "priority": 1,
      "storyPoints": 8,
      "status": "Ready",
      "type": "UserStory",
      "createdDate": "2025-01-15T10:00:00Z",
      "createdBy": "Product Owner",
      "isInCurrentSprint": true,
      "sprintId": 15
    }
  ],
  "totalCount": 1,
  "hasNext": false
}
```

### GET /api/teams/{teamId}/backlog/items/{itemId}
**Purpose**: Retrieve specific backlog item  
**Authorization**: Team member  

**Response** (200 OK):
```json
{
  "id": 25,
  "title": "User login functionality",
  "description": "As a user, I want to log into the system so that I can access my personal dashboard",
  "acceptanceCriteria": "- User can enter email and password\n- System validates credentials\n- User is redirected to dashboard on success",
  "priority": 1,
  "storyPoints": 8,
  "status": "Ready",
  "type": "UserStory",
  "createdDate": "2025-01-15T10:00:00Z",
  "lastModifiedDate": "2025-01-20T16:30:00Z",
  "createdBy": "Product Owner",
  "isInCurrentSprint": true,
  "sprintDetails": {
    "sprintId": 15,
    "sprintName": "Sprint 15",
    "addedToSprintDate": "2025-01-20T09:00:00Z"
  },
  "history": [
    {
      "date": "2025-01-20T16:30:00Z",
      "action": "Updated",
      "field": "storyPoints",
      "oldValue": "5",
      "newValue": "8",
      "modifiedBy": "Scrum Master"
    }
  ]
}
```

### POST /api/teams/{teamId}/backlog/items
**Purpose**: Create new backlog item  
**Authorization**: Product Owner  

**Request**:
```http
POST /api/teams/1/backlog/items
Content-Type: application/json

{
  "title": "Password reset functionality",
  "description": "As a user, I want to reset my password when I forget it",
  "acceptanceCriteria": "- User can request password reset via email\n- System sends secure reset link\n- User can set new password using the link",
  "type": "UserStory",
  "priority": 5
}
```

**Response** (201 Created):
```json
{
  "id": 26,
  "title": "Password reset functionality",
  "description": "As a user, I want to reset my password when I forget it",
  "acceptanceCriteria": "- User can request password reset via email\n- System sends secure reset link\n- User can set new password using the link",
  "priority": 5,
  "storyPoints": null,
  "status": "New",
  "type": "UserStory",
  "createdDate": "2025-01-27T12:00:00Z",
  "createdBy": "Product Owner"
}
```

**Validation Rules**:
- `title`: Required, 5-200 characters
- `description`: Required, 10-2000 characters
- `acceptanceCriteria`: Optional, max 5000 characters
- `type`: Required, valid enum value
- `priority`: Auto-assigned if not provided

### PUT /api/teams/{teamId}/backlog/items/{itemId}
**Purpose**: Update backlog item  
**Authorization**: Product Owner (full update) or Team member (limited fields)  

**Request**:
```http
PUT /api/teams/1/backlog/items/25
Content-Type: application/json

{
  "title": "Enhanced user login functionality",
  "description": "As a user, I want to log into the system with additional security features",
  "acceptanceCriteria": "- User can enter email and password\n- System validates credentials\n- Two-factor authentication optional\n- User is redirected to dashboard on success",
  "storyPoints": 13
}
```

### DELETE /api/teams/{teamId}/backlog/items/{itemId}
**Purpose**: Remove backlog item  
**Authorization**: Product Owner  

**Request**:
```http
DELETE /api/teams/1/backlog/items/25
```

**Response** (204 No Content)

**Business Rules**:
- Items in active sprints cannot be deleted
- Items with status "Done" cannot be deleted
- Soft delete for audit trail

## Backlog Management

### PUT /api/teams/{teamId}/backlog/reorder
**Purpose**: Reorder backlog items by priority  
**Authorization**: Product Owner  

**Request**:
```http
PUT /api/teams/1/backlog/reorder
Content-Type: application/json

{
  "itemOrders": [
    { "itemId": 25, "priority": 1 },
    { "itemId": 26, "priority": 2 },
    { "itemId": 27, "priority": 3 }
  ]
}
```

**Response** (200 OK):
```json
{
  "updatedItems": [
    { "itemId": 25, "priority": 1 },
    { "itemId": 26, "priority": 2 },
    { "itemId": 27, "priority": 3 }
  ]
}
```

### POST /api/teams/{teamId}/backlog/refine
**Purpose**: Mark backlog as refined (bulk operation)  
**Authorization**: Product Owner  

**Request**:
```http
POST /api/teams/1/backlog/refine
Content-Type: application/json

{
  "refinedDate": "2025-01-27T14:00:00Z",
  "itemIds": [25, 26, 27],
  "notes": "Estimated and clarified acceptance criteria"
}
```

### GET /api/teams/{teamId}/backlog/ready
**Purpose**: Get items ready for sprint planning  
**Authorization**: Team member  

**Response** (200 OK):
```json
{
  "readyItems": [
    {
      "id": 25,
      "title": "User login functionality",
      "storyPoints": 8,
      "priority": 1,
      "hasAcceptanceCriteria": true,
      "isEstimated": true,
      "dependencies": []
    }
  ],
  "totalReadyPoints": 42,
  "recommendedForNextSprint": [25, 26, 28]
}
```

## Estimation and Refinement

### PUT /api/teams/{teamId}/backlog/items/{itemId}/estimate
**Purpose**: Add or update story point estimate  
**Authorization**: Development Team member  

**Request**:
```http
PUT /api/teams/1/backlog/items/25/estimate
Content-Type: application/json

{
  "storyPoints": 8,
  "estimatedBy": "Development Team",
  "estimationMethod": "Planning Poker",
  "confidence": "High",
  "notes": "Includes unit tests and basic error handling"
}
```

### POST /api/teams/{teamId}/backlog/items/{itemId}/split
**Purpose**: Split large backlog item into smaller items  
**Authorization**: Product Owner  

**Request**:
```http
POST /api/teams/1/backlog/items/25/split
Content-Type: application/json

{
  "splitItems": [
    {
      "title": "Basic user login",
      "description": "Core login functionality without extras",
      "acceptanceCriteria": "- User can enter email and password\n- System validates credentials",
      "priority": 1
    },
    {
      "title": "Login security enhancements",
      "description": "Additional security features for login",
      "acceptanceCriteria": "- Two-factor authentication\n- Account lockout after failed attempts",
      "priority": 2
    }
  ]
}
```

## Reporting and Analytics

### GET /api/teams/{teamId}/backlog/metrics
**Purpose**: Get backlog health metrics  
**Authorization**: Team member  

**Response** (200 OK):
```json
{
  "totalItems": 45,
  "readyItems": 12,
  "estimatedItems": 38,
  "averageStoryPoints": 6.2,
  "velocityTrend": 28.5,
  "refinementHealth": {
    "score": 85,
    "lastRefinedDate": "2025-01-25T14:00:00Z",
    "itemsNeedingRefinement": 7
  },
  "priorityDistribution": {
    "userStories": 32,
    "bugs": 8,
    "technicalTasks": 5
  }
}
```

### GET /api/teams/{teamId}/backlog/flow
**Purpose**: Get backlog flow metrics (cumulative flow)  
**Authorization**: Team member  

**Response** (200 OK):
```json
{
  "flowData": [
    {
      "date": "2025-01-20",
      "new": 15,
      "ready": 8,
      "inProgress": 5,
      "done": 17
    }
  ],
  "leadTime": {
    "average": 12.5,
    "median": 10,
    "p95": 25
  },
  "throughput": {
    "weeklyAverage": 3.2,
    "monthlyTotal": 14
  }
}
```

---
*Contract follows OpenAPI 3.0 specification and RESTful principles*