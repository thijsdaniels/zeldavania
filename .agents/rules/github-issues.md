---
trigger: model_decision
description: When reading, writing, updating, or managing GitHub issues in the repository.
---

# GitHub Issues Management

## 1. Tool Access
- Use the **GitHub MCP Server** tools (`issue_read`, `issue_write`, `list_issues`, `search_issues`, etc.) to read, create, and update issues directly in the repository.

## 2. Issue Lifecycle & Alignment
- Always verify that the issue's scope and acceptance criteria reflect the planned deliverable before closing or referencing it in commit footers (`Closes: #<issue-number>`).
- If an issue contains extra scope that is not yet implemented (such as elemental upgrades or secondary systems), split or refine the issue first, ensuring unimplemented features are tracked in separate dedicated issues.
