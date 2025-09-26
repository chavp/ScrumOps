# Teams API Contract

**Base URL**: `/api/teams`  
**Authentication**: Required  
**Version**: v1

## Endpoints

### GET /api/teams
**Purpose**: Retrieve all teams  
**Authorization**: Any authenticated user  

**Request**:
```http
GET /api/teams
Accept: application/json
```

**Response** (200 OK):
```json
{
  "teams": [
    {
      "id": 1,
      "name": "Alpha Team",
      "description": "Mobile development team",
      "sprintLengthWeeks": 2,
      "velocity": 23.5,
      "memberCount": 5,
      "currentSprintId": 15,
      "isActive": true,
      "createdDate": "2025-01-15T10:00:00Z"
    }
  ],
  "totalCount": 1
}
```

### GET /api/teams/{id}
**Purpose**: Retrieve specific team with details  
**Authorization**: Team member or admin  

**Request**:
```http
GET /api/teams/1
Accept: application/json
```

**Response** (200 OK):
```json
{
  "id": 1,
  "name": "Alpha Team",
  "description": "Mobile development team",
  "sprintLengthWeeks": 2,
  "velocity": 23.5,
  "isActive": true,
  "createdDate": "2025-01-15T10:00:00Z",
  "members": [
    {
      "id": 10,
      "name": "John Doe",
      "email": "john@company.com",
      "role": "ProductOwner",
      "isActive": true
    }
  ],
  "currentSprint": {
    "id": 15,
    "name": "Sprint 15",
    "startDate": "2025-01-20T00:00:00Z",
    "endDate": "2025-02-03T00:00:00Z",
    "status": "Active"
  }
}
```

### POST /api/teams
**Purpose**: Create new team  
**Authorization**: Admin or Scrum Master  

**Request**:
```http
POST /api/teams
Content-Type: application/json

{
  "name": "Beta Team",
  "description": "Backend services team",
  "sprintLengthWeeks": 3
}
```

**Response** (201 Created):
```json
{
  "id": 2,
  "name": "Beta Team",
  "description": "Backend services team",
  "sprintLengthWeeks": 3,
  "velocity": 0,
  "isActive": true,
  "createdDate": "2025-01-27T12:00:00Z"
}
```

**Validation Rules**:
- `name`: Required, 3-50 characters, unique
- `description`: Optional, max 500 characters
- `sprintLengthWeeks`: Required, 1-4 weeks

### PUT /api/teams/{id}
**Purpose**: Update team details  
**Authorization**: Team Scrum Master or admin  

**Request**:
```http
PUT /api/teams/1
Content-Type: application/json

{
  "name": "Alpha Team Updated",
  "description": "Mobile and web development team",
  "sprintLengthWeeks": 2
}
```

**Response** (200 OK):
```json
{
  "id": 1,
  "name": "Alpha Team Updated",
  "description": "Mobile and web development team",
  "sprintLengthWeeks": 2,
  "velocity": 23.5,
  "isActive": true,
  "createdDate": "2025-01-15T10:00:00Z"
}
```

### DELETE /api/teams/{id}
**Purpose**: Deactivate team (soft delete)  
**Authorization**: Admin only  

**Request**:
```http
DELETE /api/teams/1
```

**Response** (204 No Content)

**Business Rules**:
- Teams with active sprints cannot be deleted
- Soft delete only (sets isActive = false)
- All team members are unassigned

## Error Responses

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "errors": {
    "name": ["Team name is required"],
    "sprintLengthWeeks": ["Sprint length must be between 1 and 4 weeks"]
  }
}
```

### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Team with ID 999 not found"
}
```

### 409 Conflict
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "Team name 'Alpha Team' already exists"
}
```

## Additional Endpoints

### GET /api/teams/{id}/members
**Purpose**: Get team members with roles  
**Response**: Array of user objects with team-specific information

### POST /api/teams/{id}/members
**Purpose**: Add member to team  
**Request**: User ID and role assignment

### GET /api/teams/{id}/velocity
**Purpose**: Get historical velocity data  
**Response**: Velocity trends over recent sprints

### GET /api/teams/{id}/metrics
**Purpose**: Get team performance metrics  
**Response**: Comprehensive team statistics and KPIs

---
*Contract follows OpenAPI 3.0 specification and RESTful principles*