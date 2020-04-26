namespace CSIT314BCE.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostCommentMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        PostId = c.Int(nullable: false),
                        Text = c.String(),
                        CreationDate = c.DateTime(),
                        DeletionDate = c.DateTime(),
                        OwnerId = c.String(nullable: false, maxLength: 128),
                        OwnerUsername = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        PostId = c.Int(nullable: false, identity: true),
                        AcceptedAnswerId = c.Int(nullable: false),
                        ParentId = c.Int(nullable: false),
                        VoteCount = c.Int(nullable: false),
                        AnswerCount = c.Int(nullable: false),
                        CreationDate = c.DateTime(),
                        DeletionDate = c.DateTime(),
                        ClosedDate = c.DateTime(),
                        Title = c.String(),
                        Body = c.String(),
                        OwnerId = c.String(nullable: false, maxLength: 128),
                        OwnerUsername = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PostId)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Posts", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Comments", "OwnerId", "dbo.AspNetUsers");
            DropIndex("dbo.Posts", new[] { "OwnerId" });
            DropIndex("dbo.Comments", new[] { "OwnerId" });
            DropTable("dbo.Posts");
            DropTable("dbo.Comments");
        }
    }
}
