# LMS Entity Relationships Documentation

## 📊 Complete Entity Relationship Diagram (Text Format)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                     IDENTITY & USER MANAGEMENT LAYER                        │
├─────────────────────────────────────────────────────────────────────────────┤
│
│  ApplicationUser (Identity)
│  ├─── UserId (PK, string)
│  ├─── Email (Required)
│  ├─── PasswordHash
│  ├─── PhoneNumber
│  ├─── FullName
│  ├─── ProfileImage
│  ├─── CreatedAt
│  │
│  └─→ Roles via IdentityUserRole
│      ├─ SuperAdmin   (Can create accounts, manage all)
│      ├─ Admin        (Can create accounts, publish announcements)
│      ├─ Instructor   (Can create courses, grade students)
│      ├─ Student      (Can enroll in courses, submit tasks)
│      └─ Parent       (Can view children's progress)
│
│  ┌──────────────────────────────────┐
│  │ 1:1 Relationships from Identity  │
│  └──────────────────────────────────┘
│
│  ApplicationUser → TbInstructor (1:1)
│  │  └─ InstructorId (PK)
│  │     ├─ Bio, Specialization, ExperienceYears
│  │     ├─ Courses (1:N → TbCourse)
│  │     └─ For users with Instructor role
│  │
│  ApplicationUser → TbStudent (1:1)
│  │  └─ StudentId (PK)
│  │     ├─ FullName, GradeId (FK)
│  │     ├─ StudentCourses (N:N → TbCourse via TbStudentCourse)
│  │     ├─ Parents (N:N → TbParent via TbParentStudent) [NEW]
│  │     ├─ StudentTests (1:N → TbStudentTest)
│  │     └─ For users with Student role
│  │
│  ApplicationUser → TbParent (1:1) [NEW]
│  │  └─ ParentId (PK)
│  │     ├─ FullName, PhoneNumber, Email
│  │     ├─ Children (N:N → TbStudent via TbParentStudent) [NEW]
│  │     ├─ Can view linked children's data
│  │     └─ For users with Parent role
│  │
│  ApplicationUser → TbAnnouncement (1:N) [NEW]
│     └─ CreatedAnnouncements
│        ├─ CreatedBy: Admin/SuperAdmin only
│        ├─ TargetAudience: Students|Parents|Instructors|All
│        └─ ViewCount tracking

└─────────────────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                      ACADEMIC CONTENT HIERARCHY                             │
├─────────────────────────────────────────────────────────────────────────────┤
│
│  TbTerm (Academic Semester/Year)
│  └─ Courses (1:N → TbCourse)
│     │
│     └─ TbCourse
│        ├─ Name, Status (Online/Recorded)
│        ├─ InstructorId (FK) → TbInstructor (1:1)
│        ├─ GradeId (FK) → TbGrade (1:1)
│        ├─ SubSubjId (FK) → TbSubSubject (1:1)
│        ├─ Announcements (Optional, 1:N → TbAnnouncement) [NEW]
│        │
│        ├─ CourseContents (1:N → TbCourseContent)
│        │  │
│        │  └─ TbCourseContent
│        │     ├─ SubContents (1:N → TbSubContent)
│        │     │  │
│        │     │  └─ TbSubContent (Lesson/Unit)
│        │     │     ├─ Sessions (1:N → TbSessionAttend)
│        │     │     ├─ Materials (1:N → TbMaterials)
│        │     │     └─ Tasks (1:N → TbTask) [From TasksController]
│        │     │        │
│        │     │        └─ TbTask
│        │     │           ├─ TaskAnswers (1:N → TbTaskAnswers)
│        │     │           │  │
│        │     │           │  └─ TbTaskAnswers
│        │     │           │     ├─ StudId (FK) → TbStudent
│        │     │           │     ├─ TaskId (FK) → TbTask
│        │     │           │     ├─ Score, Comment
│        │     │           │     └─ UpdatedBy, UpdatedDate (Grading audit)
│        │     │           │
│        │     │           └─ Parent relationship via:
│        │     │              ├─ Student.Parents → TbParentStudent
│        │     │              └─ TbParentStudent.Parent → TbParent
│        │     │
│        │     └─ Tests (Created from course content)
│        │
│        ├─ StudentCourses (N:N → TbStudent via TbStudentCourse)
│        │  │
│        │  └─ TbStudentCourse (Enrollment)
│        │     ├─ StId (FK) → TbStudent
│        │     ├─ CourseId (FK) → TbCourse
│        │     └─ Enrollment date tracking
│        │
│        ├─ Reviews (1:N → TbCourseReview)
│        ├─ Discounts (1:N → TbCourseDiscount)
│        └─ Rating, Price
│
│
│  TbGrade (Academic Level: Kindergarten, Grade 1-12, etc.)
│  ├─ Courses (1:N)
│  ├─ Students (1:N) → TbStudent
│  └─ Announcements (Optional, 1:N) [NEW]
│
│
│  TbSubject & TbSubSubject (Subject hierarchy)
│  └─ Example: Math → Algebra → Trigonometry

└─────────────────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                   PARENT-STUDENT RELATIONSHIP [NEW]                         │
├─────────────────────────────────────────────────────────────────────────────┤
│
│  TbParent (NEW)
│  ├─ ParentId (PK)
│  ├─ UserId (FK) → ApplicationUser (1:1, Unique)
│  ├─ FullName, PhoneNumber, Email
│  ├─ RelationshipType, IsPrimaryGuardian
│  │
│  └─ Children: ICollection<TbParentStudent>
│     │
│     └─ TbParentStudent (NEW - Junction Table)
│        ├─ ParentStudentId (PK)
│        ├─ ParentId (FK) → TbParent
│        ├─ StudentId (FK) → TbStudent
│        ├─ RelationshipType: "Mother", "Father", "Guardian", etc.
│        ├─ IsGuardian: bool (Legal custody)
│        ├─ ReceiveNotifications: bool
│        ├─ PermissionLevel: 
│        │  ├─ 1 = View-only (grades, attendance, progress)
│        │  ├─ 2 = View + Comment (can respond to assignments)
│        │  └─ 3 = Full access (admin-like parent)
│        ├─ LinkDate: DateTime
│        ├─ CreatedDate, UpdatedDate
│        └─ CurrentState: int (1=Active, 0=Revoked)
│           │
│           └─ Can access child's:
│              ├─ Student.StudentCourses (enrolled courses)
│              ├─ Student.StudentTests (exam results)
│              ├─ TbTaskAnswers (task submissions via Student)
│              ├─ TbSessionAttend (attendance)
│              └─ Student.Grade (academic level)
│
│
│  Parent Access Control Logic:
│  ┌─────────────────────────────────┐
│  │ Parent can view/access only if: │
│  ├─────────────────────────────────┤
│  │ 1. Linked via TbParentStudent   │
│  │ 2. CurrentState = 1 (Active)    │
│  │ 3. Relationship not revoked      │
│  │ 4. PermissionLevel allows access│
│  └─────────────────────────────────┘

└─────────────────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                    ANNOUNCEMENT SYSTEM [NEW]                                │
├─────────────────────────────────────────────────────────────────────────────┤
│
│  TbAnnouncement (NEW)
│  ├─ AnnouncementId (PK)
│  ├─ Title, Content
│  ├─ CreatedByUserId (FK) → ApplicationUser
│  │  └─ Must have role: Admin or SuperAdmin
│  │
│  ├─ TargetAudience: enum
│  │  ├─ "Students" - Visible in Student dashboard
│  │  ├─ "Parents" - Visible to linked parents only
│  │  ├─ "Instructors" - Visible in Instructor dashboard
│  │  └─ "All" - Visible to all authenticated users
│  │
│  ├─ Priority: enum
│  │  ├─ "Low" - Standard announcements
│  │  ├─ "Medium" - Important notices
│  │  ├─ "High" - Urgent alerts
│  │  └─ "Urgent" - Critical system alerts
│  │
│  ├─ Category: string
│  │  ├─ "Academic" - Course/grade related
│  │  ├─ "Event" - School events, field trips
│  │  ├─ "Holiday" - School closures, breaks
│  │  ├─ "Maintenance" - System downtime notices
│  │  ├─ "Alert" - Safety/emergency alerts
│  │  └─ "Update" - Policy/procedure updates
│  │
│  ├─ PublishedDate: DateTime (Default = Now)
│  ├─ ExpiryDate: DateTime? (Optional)
│  │  └─ If null, announcement active indefinitely
│  │
│  ├─ IsActive: bool (Default = true)
│  │  └─ Used for soft-delete or temporary hiding
│  │
│  ├─ IsPinned: bool (Default = false)
│  │  └─ Pinned announcements show at top of list
│  │
│  ├─ RequiresAcknowledgment: bool (Default = false)
│  │  └─ Important announcements may require confirmation
│  │
│  ├─ DisplayOrder: int (Default = 0)
│  │  └─ Lower number = higher priority in display
│  │
│  ├─ ViewCount: int
│  │  └─ Analytics: track engagement
│  │
│  └─ Optional relations:
│     ├─ TermId (FK) → TbTerm (term-specific announcement)
│     ├─ CourseId (FK) → TbCourse (course-specific announcement)
│     └─ GradeId (FK) → TbGrade (grade-level announcement)
│
│
│  Announcement Display Logic:
│  ┌────────────────────────────────────┐
│  │ Show announcement if:              │
│  ├────────────────────────────────────┤
│  │ 1. IsActive = true                 │
│  │ 2. CurrentState = 1 (not deleted)  │
│  │ 3. PublishedDate ≤ Now             │
│  │ 4. ExpiryDate = null OR now ≤ date │
│  │ 5. TargetAudience matches user     │
│  │ 6. If CourseId, user enrolled      │
│  │ 7. If GradeId, user in grade       │
│  │ 8. If TermId, matches current term │
│  └────────────────────────────────────┘

└─────────────────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                   ASSESSMENT & TESTING FLOW                                 │
├─────────────────────────────────────────────────────────────────────────────┤
│
│  TbTest (Created by Instructor)
│  ├─ CourseId (Implied via SubContent/Content)
│  ├─ TestQuestions (1:N → TbTestQuestion)
│  │  │
│  │  └─ TbTestQuestion
│  │     ├─ Question text, type
│  │     ├─ Choices (1:N → TbChoice)
│  │     │  │
│  │     │  └─ TbChoice
│  │     │     ├─ Choice text, isCorrect
│  │     │     └─ SelectedChoice (1:N → TbSelectedChoice)
│  │     │        └─ Student's selected choice
│  │     │
│  │     └─ CorrectAnswer tracking
│  │
│  └─ StudentTests (1:N → TbStudentTest)
│     │
│     └─ TbStudentTest
│        ├─ StudId (FK) → TbStudent
│        ├─ TestId (FK) → TbTest
│        ├─ Score, StartTime, EndTime
│        └─ SelectedChoices (1:N) for tracking answers
│
│
│  TbTask [From TasksController]
│  ├─ SubContentId (FK) → TbSubContent
│  ├─ StartDate, EndDate
│  ├─ TaskAnswers (1:N → TbTaskAnswers)
│  │  │
│  │  └─ TbTaskAnswers
│  │     ├─ TaskId (FK) → TbTask
│  │     ├─ StudId (FK) → TbStudent
│  │     ├─ AnswerURL (submission)
│  │     ├─ Score, Comment (grading)
│  │     ├─ CreatedDate (submission time)
│  │     ├─ UpdatedBy, UpdatedDate (grading audit)
│  │     └─ Late indicator via:
│  │        └─ CreatedDate > Task.EndDate
│  │
│  └─ Parent visibility:
│     ├─ Parent linked to Student
│     ├─ Student enrolled in course
│     ├─ Course has SubContent with Task
│     └─ Parent can see: AnswerURL, Score, Comment, grade

└─────────────────────────────────────────────────────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                        ATTENDANCE TRACKING                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│
│  TbSessionAttend
│  ├─ StudId (FK) → TbStudent
│  ├─ SubContentId (FK) → TbSubContent (Lesson)
│  ├─ AttendanceStatus (Present/Absent/Late)
│  ├─ Date
│  └─ Used for tracking class/lesson attendance
│
│  Parent visibility:
│  └─ Can see child's attendance for enrolled courses

└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 🔗 Key Relationship Patterns

### 1. One-to-One (1:1)
```csharp
// ApplicationUser → TbInstructor
new ParentConfiguration().Configure(modelBuilder.Entity<TbParent>())
    .HasOne(p => p.User)
    .WithOne(u => u.Parent)
    .HasForeignKey<TbParent>(p => p.UserId);

// Ensures exactly one user identity per instructor/student/parent
```

### 2. One-to-Many (1:N)
```csharp
// TbCourse → TbCourseContent
builder.HasMany(c => c.CourseContents)
    .WithOne(cc => cc.Course)
    .HasForeignKey(cc => cc.CourseId);

// One course has many content sections
```

### 3. Many-to-Many (N:N) via Junction Table
```csharp
// TbStudent ←→ TbCourse (via TbStudentCourse)
builder.HasMany(s => s.StudentCourses)
    .WithOne(sc => sc.Student)
    .HasForeignKey(sc => sc.StId);

// TbParent ←→ TbStudent (via TbParentStudent) [NEW]
builder.HasMany(p => p.Children)
    .WithOne(ps => ps.Parent)
    .HasForeignKey(ps => ps.ParentId);

// Allows multiple students per parent and multiple parents per student
```

---

## 🎯 Query Examples for Parent-Only Access

```csharp
// Example 1: Get all enrolled courses for a parent's child
var parentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var parent = await _context.Parents
    .Where(p => p.UserId == parentUserId)
    .FirstOrDefaultAsync();

var childCourses = await _context.StudentCourses
    .Where(sc => parent.Children.Any(c => c.StudentId == sc.StId))
    .Include(sc => sc.Course)
    .ToListAsync();


// Example 2: Get task submissions for a parent's child
var taskAnswers = await _context.TaskAnswers
    .Where(ta => parent.Children.Any(c => c.StudentId == ta.StudId)
              && ta.SubContentTask.SubContent.CourseContent.Course.TermId == currentTermId)
    .Include(ta => ta.SubContentTask)
    .Include(ta => ta.Student)
    .ToListAsync();


// Example 3: Get announcements visible to a parent
var announcements = await _context.Announcements
    .Where(a => a.IsActive 
            && a.CurrentState == 1
            && a.PublishedDate <= DateTime.Now
            && (a.ExpiryDate == null || DateTime.Now <= a.ExpiryDate)
            && (a.TargetAudience == "Parents" || a.TargetAudience == "All"))
    .OrderByDescending(a => a.IsPinned)
    .ThenByDescending(a => a.PublishedDate)
    .ToListAsync();
```

---

## ✅ Implementation Checklist

- [x] TbParent entity created with all required fields
- [x] TbParentStudent junction table created
- [x] TbAnnouncement entity created
- [x] TbStudent updated with Parents navigation
- [x] ApplicationUser updated with navigation properties
- [x] ParentConfiguration EF Core configuration
- [x] ParentStudentConfiguration EF Core configuration
- [x] AnnouncementConfiguration EF Core configuration
- [x] DbContext updated with DbSets
- [x] DbContext updated with configurations
- [x] All relationships properly mapped with ForeignKey attributes
- [x] All cascade delete behaviors configured
- [x] Indexes added for query performance
- [x] Audit fields (CreatedBy, UpdatedBy) included
- [x] CurrentState field for soft-delete included
- [x] Navigation properties follow EF Core conventions

---

## 📋 Migration Steps

1. **Add Migrations**
   ```powershell
   Add-Migration AddParentAndAnnouncementSystem
   ```

2. **Review the migration file** to ensure all relationships are correct

3. **Apply Migration**
   ```powershell
   Update-Database
   ```

4. **Verify** tables created in SQL Server:
   - TbParent
   - TbParentStudent
   - TbAnnouncement

---

## 🔒 Access Control Implementation

### Parent Role Access
- ✅ Can view only linked children's data
- ✅ Cannot create accounts (SuperAdmin only)
- ✅ Cannot publish announcements (Admin/SuperAdmin only)
- ✅ Cannot modify other parents' data
- ✅ Can view announcements targeted to "Parents"

### SuperAdmin Role Access
- ✅ Can create all user accounts (Admin, Instructor, Student, Parent)
- ✅ Can publish announcements
- ✅ Can create courses and manage system
- ✅ Can modify any user data

### Admin Role Access
- ✅ Can create Instructor, Student, Parent accounts
- ✅ Can publish announcements
- ✅ Can manage courses (assigned)
- ✅ Cannot create SuperAdmin

