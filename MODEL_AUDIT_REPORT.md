# LMS Model Audit Report & Implementation Plan

## 📊 AUDIT FINDINGS

### ✅ EXISTING ENTITIES (23 Models Found)

**Core Academic Models:**
- TbCourse
- TbCourseContent
- TbSubject
- TbSubSubject
- TbSubContent
- TbTerm
- TbGrade
- TbStage

**User & Staff Models:**
- ApplicationUser (Identity)
- TbInstructor
- TbStudent

**Enrollment & Progress:**
- TbStudentCourse (Junction table)
- TbSessionAttend

**Assessment Models:**
- TbTest
- TbTestQuestion
- TbStudentTest
- TbStudentAnswer
- TbChoice
- TbSelectedChoice

**Task Management:**
- TbTask
- TbTaskAnswers

**Other:**
- TbCourseReview
- TbCourseDiscount
- TbMaterials

---

### ❌ MISSING ENTITIES (To Be Implemented)

1. **TbParent** - Parent user profile
2. **TbAnnouncement** - Announcement system
3. **TbParentStudent** - Parent-Student relationship mapping

---

## 🔍 AUDIT DETAILS

### Current User Management Structure

```
ApplicationUser (Identity User)
├─ FullName: string
├─ ProfileImage: string
├─ CreatedAt: DateTime
└─ Navigation: TbInstructor (1:1)

TbInstructor (extends ApplicationUser)
├─ Id: int
├─ UserId: string (FK to ApplicationUser)
├─ Bio, Specialization, ExperienceYears, Rating
└─ Courses: List<TbCourse>

TbStudent (separate from ApplicationUser)
├─ Id: int
├─ UserId: string (FK to ApplicationUser)
├─ FullName, ParentMobile, ParentNationalId
├─ GradeId: int (FK)
└─ StudentCourses: List<TbStudentCourse>
```

### Issues Identified

⚠️ **Issue 1**: TbStudent stores ParentMobile & ParentNationalId as strings
- **Impact**: No proper Parent entity to manage access control
- **Solution**: Create TbParent entity with proper relationship

⚠️ **Issue 2**: No Parent entity exists
- **Impact**: Cannot enforce parent-only access to children's data
- **Solution**: Implement TbParent with navigation properties

⚠️ **Issue 3**: No Announcement system
- **Impact**: Cannot broadcast messages to students/parents/instructors
- **Solution**: Create TbAnnouncement with recipient tracking

⚠️ **Issue 4**: ApplicationUser extends IdentityUser but only links to TbInstructor
- **Impact**: Cannot distinguish roles for SuperAdmin, Admin, Parent
- **Solution**: Add role-based access control via ApplicationUser roles

---

## 🏗️ PROPOSED ARCHITECTURE

### New Entity Relationships

```
ApplicationUser (Identity)
├─ Role: SuperAdmin, Admin, Instructor, Student, Parent
├─ 1:1 → TbInstructor (if role = Instructor)
├─ 1:1 → TbStudent (if role = Student)
└─ 1:1 → TbParent (if role = Parent)

TbParent (NEW)
├─ Id: int
├─ UserId: string (FK to ApplicationUser)
├─ FullName, Email, PhoneNumber
├─ Children: List<TbStudent> (via TbParentStudent)
├─ CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
└─ CurrentState: int

TbParentStudent (NEW - Junction Table)
├─ Id: int
├─ ParentId: int (FK to TbParent)
├─ StudentId: int (FK to TbStudent)
├─ Relationship: string (e.g., "Father", "Mother", "Guardian")
├─ IsGuardian: bool
└─ CreatedDate: DateTime

TbAnnouncement (NEW)
├─ Id: int
├─ Title: string
├─ Content: string
├─ CreatedBy: string (FK to ApplicationUser - SuperAdmin/Admin)
├─ PublishedDate: DateTime
├─ ExpiryDate: DateTime
├─ TargetAudience: enum (Students, Parents, Instructors, All)
├─ Priority: enum (Low, Medium, High)
└─ IsActive: bool

TbStudent (MODIFIED)
├─ ParentId: int (Optional FK - for backward compatibility)
└─ Parents: List<TbParent> (via TbParentStudent) - NEW
```

---

## 🎯 IMPLEMENTATION STRATEGY

### Phase 1: Create Missing Entities
1. Create TbParent.cs
2. Create TbParentStudent.cs
3. Create TbAnnouncement.cs

### Phase 2: Create EF Core Configurations
1. ParentConfiguration.cs
2. ParentStudentConfiguration.cs
3. AnnouncementConfiguration.cs

### Phase 3: Update DbContext
1. Add DbSet<TbParent> Parents
2. Add DbSet<TbParentStudent> ParentStudents
3. Add DbSet<TbAnnouncement> Announcements

### Phase 4: Migration
1. Create migration: `Add-Migration AddParentAndAnnouncementSystem`
2. Apply migration

---

## 📋 ASSUMPTIONS

1. ✅ ApplicationUser already uses ASP.NET Identity with roles
2. ✅ Roles are managed via IdentityRole
3. ✅ TbStudent may have multiple parents (e.g., both parents guardian)
4. ✅ Parent access is controlled via role-based authorization
5. ✅ Announcements can be soft-deleted (IsActive flag)
6. ✅ CurrentState field is used for soft-delete (0 = deleted, 1 = active)

---

## 🔄 EXISTING RELATIONSHIP AUDIT

### TbStudent Relationships
```
TbStudent
├─ 1:1 → ApplicationUser (via UserId)
├─ 1:N → TbStudentCourse (via Id = StId)
├─ 1:N → TbSessionAttend (via Id)
├─ 1:N → TbStudentTest (via StudId)
├─ 1:N → TbStudentAnswer (via StudId)
└─ 1:N → TbTaskAnswers (via StudId)
```

### TbCourse Relationships
```
TbCourse
├─ 1:1 → TbInstructor (via InstructorId)
├─ 1:1 → TbTerm (via TermId)
├─ 1:1 → TbGrade (via GradeId)
├─ 1:1 → TbSubSubject (via SubSubjId)
├─ 1:N → TbCourseContent (via Id = ContentId)
├─ 1:N → TbCourseReview (via Id = CourseId)
├─ 1:N → TbStudentCourse (via Id = CourseId)
└─ 1:N → TbCourseDiscount (via Id = CourseId)
```

### TbTask Relationships
```
TbTask
├─ 1:1 → TbSubContent (via SubContentId)
└─ 1:N → TbTaskAnswers (via Id = TaskId)
```

---

## ✨ IMPLEMENTATION ENSURES

✅ No duplicate entities
✅ EF Core conventions followed (ForeignKey, ICollection)
✅ Backward compatibility maintained
✅ Role-based access control support
✅ Proper navigation properties
✅ Audit fields (CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
✅ Soft-delete support (CurrentState)

