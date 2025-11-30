# 📊 API Endpoints Visual Guide

## Controller Structure

```
┌─────────────────────────────────────────────────────────┐
│          School Management API (.NET 9)                  │
└─────────────────────────────────────────────────────────┘

├─ 🔐 AuthController
│  ├─ POST /api/auth/login
│  ├─ POST /api/auth/register
│  ├─ POST /api/auth/refresh-token
│  ├─ POST /api/auth/revoke-token
│  └─ POST /api/auth/logout-all
│
├─ 👨‍💼 Admin Controllers
│  │
│  ├─ 📚 DepartmentsController
│  │  ├─ ✅ GET /api/admin/departments → Shows all (page 1, 10 items)
│  │  ├─ ✅ GET /api/admin/departments/{id} → Shows one
│  │  ├─ POST /api/admin/departments → Create
│  │  ├─ PUT /api/admin/departments/{id} → Update
│  │  └─ DELETE /api/admin/departments/{id} → Delete
│  │
│  ├─ 📖 CoursesController
│  │  ├─ ✅ GET /api/admin/courses → Shows all (page 1, 10 items)
│  │  ├─ ✅ GET /api/admin/courses/{id} → Shows one
│  │  ├─ POST /api/admin/courses → Create
│  │  ├─ PUT /api/admin/courses/{id} → Update
│  │  └─ DELETE /api/admin/courses/{id} → Delete
│  │
│  └─ 👥 UsersController
│     ├─ ✅ GET /api/admin/users → Shows all (page 1, 10 items)
│     ├─ ✅ GET /api/admin/users/{id} → Shows one
│     ├─ POST /api/admin/users → Create
│     ├─ PUT /api/admin/users/{id} → Update
│     └─ DELETE /api/admin/users/{id} → Delete
│
├─ 🏫 Teacher Controllers
│  │
│  ├─ 🎓 ClassesController
│  │  ├─ ✅ GET /api/teacher/classes → Shows teacher's classes
│  │  ├─ ✅ GET /api/teacher/classes/{id} → Shows one class
│  │  ├─ POST /api/teacher/classes → Create
│  │  ├─ PUT /api/teacher/classes/{id} → Update
│  │  └─ POST /api/teacher/classes/{id}/students → Assign students
│  │
│  ├─ 📋 AttendanceController
│  │  └─ POST /api/teacher/attendance → Mark attendance
│  │
│  └─ 📝 AssignmentsController
│     ├─ ✅ GET /api/teacher/assignments/class/{classId} → List assignments
│     ├─ POST /api/teacher/assignments → Create
│     └─ POST /api/teacher/assignments/submissions/{id}/grade → Grade
│
└─ 👨‍🎓 Student Controllers
   │
   └─ 📚 StudentController
      ├─ ✅ GET /api/student/classes → Shows enrolled classes
      ├─ ✅ GET /api/student/attendance → Shows attendance records
      ├─ ✅ GET /api/student/grades → Shows graded submissions
      ├─ POST /api/student/submissions/{id}/submit → Submit work
      └─ GET /api/student/submissions/assignment/{id} → View submissions
```

---

## GET Endpoints Summary

### ✅ All These Show Data Instantly

```
Admin Level:
┌─────────────────────────────────────────┐
│ GET /api/admin/departments              │ → First 10
│ GET /api/admin/courses                  │ → First 10
│ GET /api/admin/users                    │ → First 10
└─────────────────────────────────────────┘

Teacher Level:
┌─────────────────────────────────────────┐
│ GET /api/teacher/classes                │ → First 10
│ GET /api/teacher/assignments/class/{id} │ → First 10
└─────────────────────────────────────────┘

Student Level:
┌─────────────────────────────────────────┐
│ GET /api/student/classes                │ → First 10
│ GET /api/student/attendance             │ → All records
│ GET /api/student/grades                 │ → All records
└─────────────────────────────────────────┘
```

---

## Data Flow Diagram

```
User
  ↓
[Login] → Get JWT Token
  ↓
[Use Token in Headers]
  ↓
[Call GET Endpoint with NO parameters]
  ↓
[API uses defaults: page=1, pageSize=10]
  ↓
[Database query with defaults]
  ↓
[Returns paginated response]
  ↓
[User sees data immediately] ✅
```

---

## Request/Response Flow

### Simple GET Request
```
Request:
┌──────────────────────────────────────────┐
│ GET /api/admin/departments               │
│ Authorization: Bearer {token}            │
│ (No other parameters needed!)            │
└──────────────────────────────────────────┘

Processing:
  - No parameters? Use defaults
  - page = 1
  - pageSize = 10
  - filter = null
  - Query database

Response:
┌──────────────────────────────────────────┐
│ {                                        │
│   "page": 1,                             │
│   "pageSize": 10,                        │
│   "total": 25,                           │
│   "items": [                             │
│     { "id": "...", "name": "..." },     │
│     { "id": "...", "name": "..." },     │
│     ...                                  │
│   ]                                      │
│ }                                        │
└──────────────────────────────────────────┘
```

---

## HTTP Methods Overview

```
GET     → Read data (✅ No params needed)
┌─ GET /api/admin/departments
├─ GET /api/admin/departments/123
├─ GET /api/student/classes
└─ GET /api/student/attendance

POST    → Create data (Requires body)
├─ POST /api/admin/departments { name: "..." }
├─ POST /api/auth/login { email, password }
└─ POST /api/student/submissions/submit

PUT     → Update data (Requires ID + body)
├─ PUT /api/admin/departments/123 { name: "..." }
└─ PUT /api/admin/courses/456 { ... }

DELETE  → Remove data (Requires ID)
├─ DELETE /api/admin/departments/123
└─ DELETE /api/admin/courses/456
```

---

## Default Values Applied

```
When you GET without parameters:

┌─────────────────────┬─────────┬──────────────┐
│ Parameter           │ Default │ Type         │
├─────────────────────┼─────────┼──────────────┤
│ page                │ 1       │ int          │
│ pageSize            │ 10      │ int          │
│ filter              │ null    │ string?      │
│ departmentId        │ null    │ Guid?        │
│ classId             │ null    │ Guid?        │
│ from (date)         │ null    │ DateTime?    │
│ to (date)           │ null    │ DateTime?    │
│ assignmentId        │ null    │ Guid?        │
└─────────────────────┴─────────┴──────────────┘

Result: Always shows something useful!
```

---

## Swagger UI Flow

```
┌─────────────────────────────────────────┐
│ 1. Open Swagger: https://localhost:5001 │
└─────────────────────────────────────────┘
        ↓
┌─────────────────────────────────────────┐
│ 2. Click "Authorize" button (top right) │
└─────────────────────────────────────────┘
        ↓
┌─────────────────────────────────────────┐
│ 3. Paste Token: Bearer {token}          │
└─────────────────────────────────────────┘
        ↓
┌─────────────────────────────────────────┐
│ 4. Find GET endpoint you want           │
└─────────────────────────────────────────┘
        ↓
┌─────────────────────────────────────────┐
│ 5. Click "Try it out"                   │
└─────────────────────────────────────────┘
        ↓
┌─────────────────────────────────────────┐
│ 6. Leave parameters empty               │
└─────────────────────────────────────────┘
        ↓
┌─────────────────────────────────────────┐
│ 7. Click "Execute"                      │
└─────────────────────────────────────────┘
        ↓
┌─────────────────────────────────────────┐
│ 8. ✅ See data immediately!             │
└─────────────────────────────────────────┘
```

---

## User Role Access Matrix

```
                    Admin    Teacher   Student
┌──────────────────────────────────────────────┐
│ /admin/departments │  ✅      ❌       ❌    │
│ /admin/courses     │  ✅      ❌       ❌    │
│ /admin/users       │  ✅      ❌       ❌    │
│ /teacher/classes   │  ❌      ✅       ❌    │
│ /teacher/attend..  │  ❌      ✅       ❌    │
│ /teacher/assign..  │  ❌      ✅       ❌    │
│ /student/classes   │  ❌      ❌       ✅    │
│ /student/attend..  │  ❌      ❌       ✅    │
│ /student/grades    │  ❌      ❌       ✅    │
└──────────────────────────────────────────────┘
```

---

## Error Handling

```
Request fails?

┌──────────────────────────────────────────┐
│ 401 Unauthorized                         │
│ → Token missing or invalid               │
│ → Solution: Add Authorization header     │
└──────────────────────────────────────────┘

┌──────────────────────────────────────────┐
│ 403 Forbidden                            │
│ → Wrong role for endpoint                │
│ → Solution: Login with correct user      │
└──────────────────────────────────────────┘

┌──────────────────────────────────────────┐
│ 404 Not Found                            │
│ → ID doesn't exist                       │
│ → Solution: Check the ID                 │
└──────────────────────────────────────────┘

┌──────────────────────────────────────────┐
│ 500 Server Error                         │
│ → Something went wrong                   │
│ → Solution: Check console logs           │
└──────────────────────────────────────────┘
```

---

## Performance Characteristics

```
┌─────────────────────────────────────────┐
│ Feature              │ Status           │
├─────────────────────────────────────────┤
│ Pagination          │ ✅ (page 1-10)   │
│ Filtering           │ ✅ (search)      │
│ Caching             │ ✅ (10 min TTL)  │
│ Async/Await         │ ✅ (no blocking) │
│ Database Indexing   │ ✅ (optimized)   │
│ Connection Pool     │ ✅ (EF Core)     │
│ Compression         │ ✅ (gzip)        │
└─────────────────────────────────────────┘
```

---

## Summary

✅ **8 GET endpoints** → Show data without parameters
✅ **30+ total endpoints** → Full CRUD operations
✅ **Default values** → Smart defaults applied
✅ **Role-based access** → Secure authorization
✅ **Pagination** → Handle large datasets
✅ **Filtering** → Optional parameter support
✅ **Error handling** → Clear error messages

**All working together seamlessly!** 🚀

---

## Quick Start

```bash
# 1. Run API
dotnet run --project project1

# 2. Open browser
https://localhost:5001/swagger

# 3. Click GET endpoint

# 4. Click Execute

# 5. ✅ See data!

Done! No parameters needed! 🎉
```

```bash
dotnet ef migrations add NameOfMigration --project project1 --startup-project project1 --output-dir Infrastructure/Data/Migrations
dotnet ef database update --project project1 --startup-project project1
