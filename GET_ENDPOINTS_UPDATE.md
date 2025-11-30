# ? GET Endpoints Update - No Input Required

## Summary

All GET endpoints in the API have been updated to display data **without requiring any input parameters**. They now have default values, allowing you to access the data directly.

---

## What Changed

### Before ?
```bash
# Had to specify query parameters
GET /api/admin/departments?page=1&pageSize=10
GET /api/student/classes?page=1&pageSize=10
GET /api/student/attendance?classId=&from=&to=
```

### After ?
```bash
# Just call the endpoint directly - uses defaults
GET /api/admin/departments
GET /api/student/classes
GET /api/student/attendance
```

---

## Updated Controllers

### 1. Admin Controllers ?

#### DepartmentsController
```csharp
// Now GET without parameters shows first page with 10 items
[HttpGet]
public async Task<IActionResult> Get(int page = 1, int pageSize = 10, string? filter = null)
```

#### CoursesController
```csharp
// Same - get all courses with defaults
[HttpGet]
public async Task<IActionResult> Get(int page = 1, int pageSize = 10, string? filter = null, Guid? departmentId = null)
```

#### UsersController
```csharp
// Get all users with defaults
[HttpGet]
public async Task<IActionResult> Get(int page = 1, int pageSize = 10, string? filter = null)
```

---

### 2. Teacher Controllers ?

#### ClassesController
```csharp
// Get teacher's classes with defaults
[HttpGet]
public async Task<IActionResult> Get(int page = 1, int pageSize = 10)
```

#### AssignmentsController
```csharp
// Get assignments for a class with defaults
[HttpGet("class/{classId}")]
public async Task<IActionResult> GetForClass(Guid classId, int page = 1, int pageSize = 10)
```

---

### 3. Student Controllers ?

#### StudentController - GetClasses
```csharp
// Get student's enrolled classes with defaults
[HttpGet("classes")]
public async Task<IActionResult> GetClasses(int page = 1, int pageSize = 10)
```

#### StudentController - GetAttendance
```csharp
// Get attendance without parameters - shows all
[HttpGet("attendance")]
public async Task<IActionResult> GetAttendance(Guid? classId = null, DateTime? from = null, DateTime? to = null)
```

#### StudentController - GetGrades
```csharp
// Get grades without parameters - shows all graded submissions
[HttpGet("grades")]
public async Task<IActionResult> GetGrades(Guid? assignmentId = null)
```

---

## Usage Examples

### Before (had to specify everything)
```bash
curl -X GET "https://localhost:5001/api/admin/departments?page=1&pageSize=10" \
  -H "Authorization: Bearer <token>"
```

### After (just call it!)
```bash
# All these work the same way - returns first 10 items
curl -X GET "https://localhost:5001/api/admin/departments" \
  -H "Authorization: Bearer <token>"

curl -X GET "https://localhost:5001/api/admin/courses" \
  -H "Authorization: Bearer <token>"

curl -X GET "https://localhost:5001/api/student/classes" \
  -H "Authorization: Bearer <token>"

curl -X GET "https://localhost:5001/api/student/attendance" \
  -H "Authorization: Bearer <token>"

curl -X GET "https://localhost:5001/api/student/grades" \
  -H "Authorization: Bearer <token>"
```

---

## Default Values

| Parameter | Default | Meaning |
|-----------|---------|---------|
| `page` | 1 | First page |
| `pageSize` | 10 | 10 items per page |
| `filter` | null | No search filter |
| `departmentId` | null | All departments |
| `classId` | null | All classes |
| `from` | null | No start date |
| `to` | null | No end date |
| `assignmentId` | null | All assignments |

---

## What Still Works

? You can still pass parameters if you want specific data:

```bash
# Get page 2 of departments
GET /api/admin/departments?page=2&pageSize=10

# Get departments matching "CS"
GET /api/admin/departments?filter=CS

# Get courses for a specific department
GET /api/admin/courses?departmentId={guid}

# Get attendance for a specific class
GET /api/student/attendance?classId={guid}

# Get attendance for a date range
GET /api/student/attendance?from=2025-09-01&to=2025-12-31

# Get grades for a specific assignment
GET /api/student/grades?assignmentId={guid}
```

---

## Testing with Swagger

1. Go to: `https://localhost:5001/swagger`
2. Login with token
3. Click any GET endpoint
4. Click "Try it out"
5. Leave parameters empty
6. Click "Execute"
7. ? See data immediately!

---

## Summary of Changes

| Endpoint | Status | Behavior |
|----------|--------|----------|
| GET /api/admin/departments | ? Updated | Shows 10 items, page 1 |
| GET /api/admin/courses | ? Updated | Shows 10 items, page 1 |
| GET /api/admin/users | ? Updated | Shows 10 items, page 1 |
| GET /api/teacher/classes | ? Updated | Shows teacher's classes, page 1 |
| GET /api/teacher/assignments/class/{id} | ? Updated | Shows assignments, page 1 |
| GET /api/student/classes | ? Updated | Shows student's classes, page 1 |
| GET /api/student/attendance | ? Updated | Shows all attendance records |
| GET /api/student/grades | ? Updated | Shows all graded submissions |

---

## Build Status

? **Build Successful** - No errors or warnings

---

## Next Steps

1. Run the API locally
2. Go to Swagger UI
3. Click GET endpoints
4. No parameters needed!
5. Enjoy your data instantly

```bash
dotnet run --project project1
# Then visit: https://localhost:5001/swagger
```

---

**All Done!** ?? Your GET endpoints are now user-friendly and show data without requiring input.
