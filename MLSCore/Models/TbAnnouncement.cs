using MLSCore.IdentityModel;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MLSCore.Models
{
    /// <summary>
    /// TbAnnouncement: System-wide announcements visible to students, parents, and instructors.
    /// Only SuperAdmin and Admin roles can create/publish announcements.
    /// 
    /// Target Audiences:
    /// - Students: General announcements, academic news, schedule changes
    /// - Parents: Student progress, events, school closures
    /// - Instructors: Staff announcements, policy updates, training
    /// - All: System-wide announcements
    /// </summary>
    public class TbAnnouncement
    {
        public int Id { get; set; }

        /// <summary>
        /// Title of the announcement (e.g., "School Closure - Holidays")
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// Detailed content/body of the announcement
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Additional description/summary
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Target audience for this announcement
        /// Values: "Students", "Parents", "Instructors", "All"
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TargetAudience { get; set; } = "All";

        /// <summary>
        /// Priority level of the announcement
        /// Values: "Low", "Medium", "High", "Urgent"
        /// </summary>
        [StringLength(20)]
        public string Priority { get; set; } = "Medium";

        /// <summary>
        /// Category/Type of announcement
        /// Examples: "Academic", "Event", "Holiday", "Maintenance", "Alert", "Update"
        /// </summary>
        [StringLength(50)]
        public string Category { get; set; }

        /// <summary>
        /// Image/thumbnail URL for the announcement
        /// </summary>
        [StringLength(500)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// URL to attached file or external resource (optional)
        /// </summary>
        [StringLength(500)]
        public string AttachmentUrl { get; set; }

        /// <summary>
        /// Date and time the announcement was published
        /// </summary>
        public DateTime PublishedDate { get; set; }

        /// <summary>
        /// Date and time after which the announcement should not be displayed
        /// If null, announcement remains active indefinitely
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Whether the announcement is currently active/visible
        /// Used for soft-delete or temporarily hiding announcements
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Whether this announcement should be displayed prominently/pinned
        /// </summary>
        public bool IsPinned { get; set; } = false;

        /// <summary>
        /// Whether users need to acknowledge/confirm reading this announcement
        /// </summary>
        public bool RequiresAcknowledgment { get; set; } = false;

        /// <summary>
        /// Display order for announcements (lower number = higher priority in display)
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Current state (1=Active, 0=Deleted/Archived)
        /// </summary>
        public int CurrentState { get; set; } = 1;

        /// <summary>
        /// Audit: Who created this announcement (typically Admin/SuperAdmin user ID)
        /// </summary>
        [StringLength(256)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Audit: When this announcement was created
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Audit: Who last updated this announcement
        /// </summary>
        [StringLength(256)]
        public string UpdatedBy { get; set; }

        /// <summary>
        /// Audit: When this announcement was last updated
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Foreign key to ApplicationUser (who created/published the announcement)
        /// </summary>
        [ForeignKey("Creator")]
        public string CreatedByUserId { get; set; }

        /// <summary>
        /// Navigation property to creator (Admin/SuperAdmin user)
        /// </summary>
        public ApplicationUser Creator { get; set; }

        /// <summary>
        /// Number of times this announcement has been viewed (for analytics)
        /// </summary>
        public int ViewCount { get; set; } = 0;

        /// <summary>
        /// Optional: Related term/semester (if announcement is specific to a term)
        /// </summary>
        [ForeignKey("Term")]
        public int? TermId { get; set; }
        public TbTerm Term { get; set; }

        /// <summary>
        /// Optional: Related course (if announcement is course-specific)
        /// </summary>
        [ForeignKey("Course")]
        public int? CourseId { get; set; }
        public TbCourse Course { get; set; }

        /// <summary>
        /// Optional: Related grade (if announcement is grade-specific)
        /// </summary>
        [ForeignKey("Grade")]
        public int? GradeId { get; set; }
        public TbGrade Grade { get; set; }
    }
}
