# Contributing to Candy Crush Match-3 Game

## Code of Conduct

Be respectful, inclusive, and constructive in all interactions.

## Getting Started

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Make your changes
4. Write tests for new features
5. Submit a pull request

## Development Workflow

### Setting Up Local Environment

```bash
git clone https://github.com/atgnews/candy-crush-match3-game.git
cd candy-crush-match3-game
unity -createProject . -projectName CandyCrush
```

### Creating a Feature Branch

```bash
git checkout -b feature/game-name
git push -u origin feature/game-name
```

### Committing Code

```bash
# Good commit messages
[FEATURE] Implement match detection engine
[BUGFIX] Fix cascade animation timing
[DOCS] Update README
[REFACTOR] Simplify grid system

# Use present tense
# Keep commits focused and atomic
```

## Code Style Guide

### C# Style

```csharp
// Classes
public class GameplayController : MonoBehaviour
{
    // Private fields with underscore
    private int score;
    [SerializeField] private GridSystem gridSystem;

    // Public properties
    public int Score => score;

    // Methods
    public void Initialize()
    {
        // Implementation
    }

    private void UpdateScore(int amount)
    {
        score += amount;
    }
}
```

### Naming Conventions

- Classes: PascalCase (e.g., `GameplayController`)
- Methods: PascalCase (e.g., `InitializeGame`)
- Variables: camelCase (e.g., `playerScore`)
- Constants: UPPER_CASE (e.g., `GRID_WIDTH`)
- Private fields: _camelCase or camelCase (e.g., `_gridSystem`)
- SerializeFields: camelCase (e.g., `gridSystem`)

## Testing Requirements

Before submitting a PR:

1. Write unit tests for new features
2. Run all tests: `npm test`
3. Achieve > 80% code coverage
4. Test on at least 2 devices
5. Verify no performance regressions

## Pull Request Process

1. **Create PR** with descriptive title
2. **Description** - Explain what and why
3. **Link Issues** - Reference related issues
4. **Screenshots** - Include before/after for UI changes
5. **Testing** - Describe testing done
6. **Wait for Review** - Respond to feedback promptly
7. **Merge** - Squash commits if needed

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Performance improvement
- [ ] Documentation

## Testing
How was this tested?

## Screenshots
(if applicable)

## Checklist
- [ ] Code follows style guide
- [ ] Self-reviewed code
- [ ] Tests added/updated
- [ ] Documentation updated
- [ ] No breaking changes
```

## Performance Guidelines

- Keep methods focused and short
- Use object pooling for frequently created objects
- Avoid allocations in Update()
- Use async/await for long operations
- Profile before and after changes

## Documentation

- Add XML comments to public methods
- Update README for major changes
- Add inline comments for complex logic
- Keep documentation up-to-date

## Reporting Issues

Use GitHub Issues with clear:
- Title
- Description
- Steps to reproduce
- Expected vs actual behavior
- Screenshots/logs
- Device and OS info

## Code Review Process

1. At least 2 approvals required
2. All CI checks must pass
3. Address all feedback
4. Rebase with main branch
5. Delete branch after merge

## Release Process

See BUILD_GUIDE.md for detailed release procedures.

## Questions?

Open an issue or contact the team.

Thank you for contributing!
