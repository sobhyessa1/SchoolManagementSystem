# Quick Reference Guide - API Endpoints

## Authentication

```bash
# Register (Student public registration)
POST /api/auth/register
{
  "name": "John Doe",
  "email": "john@school.test",
  "password": "SecurePass123!",
  "role": "Student"
}

# Login
POST /api/auth/login
{
  "email": "user@school.test",
  "password": "password"
}
? Returns: { accessToken, refreshToken, expiresIn }

# Refresh Token
POST /api/auth/refresh-token
{ "refreshToken": "..." }

# Revoke Single Token
POST /api/auth/revoke-token
{ "token": "..." }

# Logout All Devices
POST /api/auth/logout-all
(Requires Authorization header)
```

---

## Admin Endpoints

### Departments
```bash
GET    /api/admin/departments?page=1&pageSize=10&filter=CS
GET    /api/admin/departments/{id}
POST   /api/admin/departments
{ "name": "CS", "description": "...", "headOfDepartmentId": null }
PUT    /api/admin/departments/{id}
DELETE /api/admin/departments/{id}
```

### Courses
```bash
GET    /api/admin/courses?page=1&pageSize=10&filter=CS101&departmentId={id}
GET    /api/admin/courses/{id}
POST   /api/admin/courses
{
  "name": "Programming 101",
  "code": "CS101",
  "description": "...",
  "departmentId": "{guid}",
  "credits": 3
}
PUT    /api/admin/courses/{id}
DELETE /api/admin/courses/{id}
```

### Users (Admin Only)
```bash
GET    /api/admin/users?page=1&pageSize=10&filter=Smith
GET    /api/admin/users/{id}
POST   /api/admin/users
{
  "name": "Dr. Smith",
  "email": "smith@school.test",
  "password": "SecurePass123!",
  "role": "Teacher"  # or "Admin"
}
PUT    /api/admin/users/{id}
DELETE /api/admin/users/{id}
```

---

## Teacher Endpoints

### Classes
```bash
GET    /api/teacher/classes?page=1&pageSize=10
GET    /api/teacher/classes/{id}
POST   /api/teacher/classes
{
  "name": "CS101-Fall2025",
  "courseId": "{guid}",
  "teacherId": "{guid}",
  "semester": 1,
  "startDate": "2025-09-01T00:00:00Z",
  "endDate": "2025-12-31T00:00:00Z"
}
PUT    /api/teacher/classes/{id}
POST   /api/teacher/classes/{id}/students
["{studentId1}", "{studentId2}"]
```

### Attendance
```bash
POST   /api/teacher/attendance
{
  "classId": "{guid}",
  "date": "2025-11-25T00:00:00Z",
  "entries": [
    {
      "studentId": "{guid}",
      "status": "Present"  # or "Absent", "Late", "Excused"
    }
  ]
}
```

### Assignments
```bash
GET    /api/teacher/assignments/class/{classId}?page=1&pageSize=10
POST   /api/teacher/assignments
{
  "classId": "{guid}",
  "title": "Assignment 1",
  "description": "...",
  "dueDate": "2025-12-15T23:59:00Z"
}
POST   /api/teacher/assignments/submissions/{submissionId}/grade
{
  "grade": 95.5,
  "remarks": "Excellent work!"
}
```

---

## Student Endpoints

### Classes (Enrolled)
```bash
GET    /api/student/classes?page=1&pageSize=10
```

### Attendance
```bash
GET    /api/student/attendance?classId={id}&from=2025-09-01&to=2025-12-31
```

### Submissions
```bash
POST   /api/student/submissions/{assignmentId}/submit
(multipart/form-data)
- file: (File, max 5MB, .pdf .doc .docx .zip .txt)

GET    /api/student/submissions/assignment/{assignmentId}
```

### Grades
```bash
GET    /api/student/grades?assignmentId={id}
```

---

## Status Codes

| Code | Meaning |
|------|---------|
| 200 | OK (GET success) |
| 201 | Created (POST success) |
| 204 | No Content (PUT/DELETE success) |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (invalid token) |
| 403 | Forbidden (insufficient role/permission) |
| 404 | Not Found |
| 500 | Server Error |

---

## Default Headers

```bash
# All requests (except login/register)
Authorization: Bearer {accessToken}
Content-Type: application/json

# Pagination Response Header
X-Pagination: {"page":1,"pageSize":10,"total":100,"totalPages":10}
```

---

## Common Attendance Statuses

- `Present`
- `Absent`
- `Late`
- `Excused`

---

## Pagination Example

```bash
# Get page 2 with 5 items per page
GET /api/admin/departments?page=2&pageSize=5

# Response includes:
X-Pagination: {"page":2,"pageSize":5,"total":25,"totalPages":5}
[
  { "id": "...", "name": "...", ... },
  ...
]
```

---

## Common Errors

```bash
# Validation Error (400)
{
  "error": "Department name is required"
}

# Unauthorized (401)
{
  "message": "Unauthorized"
}

# Forbidden (403)
{
  "message": "Forbidden"
}

# Not Found (404)
{
  "error": "Resource not found"
}
```

---

## Password Requirements

Minimum 8 characters with:
- ? Uppercase letter (A-Z)
- ? Lowercase letter (a-z)
- ? Digit (0-9)
- ? Special character (!@#$%^&*)

Example: `SecurePass123!`

---

## File Upload Rules

- **Max Size**: 5 MB
- **Allowed Types**: .pdf, .doc, .docx, .zip, .txt
- **Storage**: `/uploads/{guid}.{ext}`
- **Access**: `GET /uploads/{filename}`

---

## Token Expiry

- **Access Token**: 15 minutes
- **Refresh Token**: 7 days

---

## User Roles

1. **Admin**
   - Create/manage departments
   - Create/manage courses
   - Create/manage users
   - Full system access

2. **Teacher**
   - Create classes
   - Assign students to classes
   - Create assignments
   - Mark attendance
   - Grade submissions

3. **Student**
   - View enrolled classes
   - View attendance
   - Submit assignments
   - View grades

---

## Quick Login Flow

```bash
# 1. Login
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@school.test","password":"pass"}'

# Response: { "accessToken": "eyJ...", "refreshToken": "...", "expiresIn": 900 }

# 2. Use token in requests
curl -X GET https://localhost:5001/api/student/classes \
  -H "Authorization: Bearer eyJ..."

# 3. Refresh token when expired
curl -X POST https://localhost:5001/api/auth/refresh-token \
  -H "Content-Type: application/json" \
  -d '{"refreshToken": "..."}'
```

---

## Swagger/OpenAPI

Access: `https://localhost:5001/swagger`

- Interactive API documentation
- Try-out functionality with JWT support
- Schema definitions for all endpoints

---

## Environment Variables

```
Jwt:Key=YourSecretKeyHere...
Jwt:Issuer=project1
Jwt:Audience=project1_users
ConnectionStrings:DefaultConnection=Data Source=...;Initial Catalog=school;...
Redis:ConnectionString=redis-server:6379  (optional)
```

---

*Last Updated: November 25, 2025*
