# ?? GET Endpoints Quick Demo

## Swagger UI - Easiest Way

### Step 1: Start API
```bash
cd project1
dotnet run
```

### Step 2: Open Swagger
```
https://localhost:5001/swagger
```

### Step 3: Login
1. Find "auth" section
2. Click "POST /api/auth/login"
3. Click "Try it out"
4. Use default credentials:
   ```json
   {
     "email": "admin@school.test",
     "password": "Admin@123"
   }
   ```
5. Copy the `accessToken`

### Step 4: Authorize
1. Click "Authorize" button (top right)
2. Paste token: `Bearer {your-token}`
3. Click "Authorize"

### Step 5: Try GET Endpoints
Just click any GET endpoint and click "Execute" - **No parameters needed!**

---

## All GET Endpoints (Updated)

### ?? Admin - Departments
```
GET /api/admin/departments
```
Shows: First 10 departments (page 1)

### ?? Admin - Courses
```
GET /api/admin/courses
```
Shows: First 10 courses (page 1)

### ?? Admin - Users
```
GET /api/admin/users
```
Shows: First 10 users (page 1)

### ?? Teacher - Classes
```
GET /api/teacher/classes
```
Shows: Your classes (page 1)

### ?? Teacher - Assignments
```
GET /api/teacher/assignments/class/{classId}
```
Shows: Assignments for that class (page 1)

### ?? Student - Classes
```
GET /api/student/classes
```
Shows: Your enrolled classes (page 1)

### ?? Student - Attendance
```
GET /api/student/attendance
```
Shows: Your attendance records (all)

### ?? Student - Grades
```
GET /api/student/grades
```
Shows: Your graded submissions (all)

---

## Command Line Examples

### 1?? Login First
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@school.test","password":"Admin@123"}'

# Save the accessToken from response
```

### 2?? Get Departments (No parameters!)
```bash
curl -X GET https://localhost:5001/api/admin/departments \
  -H "Authorization: Bearer {YOUR_TOKEN}"
```

### 3?? Get Courses
```bash
curl -X GET https://localhost:5001/api/admin/courses \
  -H "Authorization: Bearer {YOUR_TOKEN}"
```

### 4?? Get Users
```bash
curl -X GET https://localhost:5001/api/admin/users \
  -H "Authorization: Bearer {YOUR_TOKEN}"
```

### 5?? Get Student's Classes (use student token)
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"student1@school.test","password":"Student@123"}'

# Then:
curl -X GET https://localhost:5001/api/student/classes \
  -H "Authorization: Bearer {STUDENT_TOKEN}"
```

### 6?? Get Attendance
```bash
curl -X GET https://localhost:5001/api/student/attendance \
  -H "Authorization: Bearer {STUDENT_TOKEN}"
```

### 7?? Get Grades
```bash
curl -X GET https://localhost:5001/api/student/grades \
  -H "Authorization: Bearer {STUDENT_TOKEN}"
```

---

## Response Example

All GET endpoints return data in this format:

### List Response (with pagination)
```json
{
  "page": 1,
  "pageSize": 10,
  "total": 25,
  "items": [
    {
      "id": "00000000-0000-0000-0000-000000000001",
      "name": "Computer Science",
      "description": "Department of Computer Science",
      "createdDate": "2025-11-25T10:00:00Z"
    },
    {
      "id": "00000000-0000-0000-0000-000000000002",
      "name": "Mathematics",
      "description": "Department of Mathematics",
      "createdDate": "2025-11-25T10:01:00Z"
    }
  ]
}
```

### Single Item Response
```json
{
  "id": "00000000-0000-0000-0000-000000000001",
  "name": "Computer Science",
  "description": "Department of Computer Science",
  "createdDate": "2025-11-25T10:00:00Z"
}
```

---

## Optional: Add Parameters for Filtering

Even though defaults work, you can still add parameters:

```bash
# Get page 2
curl -X GET "https://localhost:5001/api/admin/departments?page=2" \
  -H "Authorization: Bearer {TOKEN}"

# Search for something
curl -X GET "https://localhost:5001/api/admin/departments?filter=CS" \
  -H "Authorization: Bearer {TOKEN}"

# Get more items per page
curl -X GET "https://localhost:5001/api/admin/departments?pageSize=20" \
  -H "Authorization: Bearer {TOKEN}"

# Combine them
curl -X GET "https://localhost:5001/api/admin/courses?page=2&pageSize=20&departmentId={guid}" \
  -H "Authorization: Bearer {TOKEN}"
```

---

## ?? Learning Path

### Beginner: Use Swagger
1. Start API
2. Go to Swagger UI
3. Click GET endpoint
4. Click Execute
5. See data!

### Intermediate: Use curl
1. Get token
2. Copy paste curl command
3. Run in terminal
4. Parse JSON response

### Advanced: Postman/Insomnia
1. Import API
2. Authenticate
3. Click request
4. See results

---

## Default Test Users

### Admin Account
- **Email**: admin@school.test
- **Password**: Admin@123
- **Can see**: Departments, Courses, Users, All data

### Teacher Account
- **Email**: teacher1@school.test
- **Password**: Teacher@123
- **Can see**: Classes, Assignments, Student grades

### Student Account
- **Email**: student1@school.test
- **Password**: Student@123
- **Can see**: Own classes, attendance, grades

---

## Status Codes Returned

| Code | Meaning | Example |
|------|---------|---------|
| 200 | Success | Data returned |
| 401 | Unauthorized | Missing/invalid token |
| 403 | Forbidden | Wrong role |
| 404 | Not found | ID doesn't exist |
| 500 | Server error | Something went wrong |

---

## Troubleshooting

### "Authorization has been denied"
? Add token header: `Authorization: Bearer {token}`

### "No items returned"
? Check if you're using correct role (Admin sees everything, Teacher sees own classes, etc.)

### "404 Not Found"
? Make sure endpoint path is correct

### "500 Server Error"
? Check logs in console

---

## Summary Table

| Want to See | Endpoint | Parameters | Default Shows |
|------------|----------|-----------|----------------|
| All Departments | `/api/admin/departments` | Optional | Page 1, 10 items |
| All Courses | `/api/admin/courses` | Optional | Page 1, 10 items |
| All Users | `/api/admin/users` | Optional | Page 1, 10 items |
| Your Classes (teacher) | `/api/teacher/classes` | Optional | Page 1, 10 items |
| Assignments | `/api/teacher/assignments/class/{id}` | Optional | Page 1, 10 items |
| Your Classes (student) | `/api/student/classes` | Optional | Page 1, 10 items |
| Your Attendance | `/api/student/attendance` | Optional | All records |
| Your Grades | `/api/student/grades` | Optional | All records |

---

**All endpoints ready to use!** ??

Just call them without any parameters and get instant data!
