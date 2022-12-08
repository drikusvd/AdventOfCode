namespace AdventOfCode2022.Day8;

public class Day8Processor: ProcessorBase
{
    protected override void Silver(string[] lines)
    {
        var visibleCount = 0;
        for (var row = 0; row < lines.Length; row++)
        {
            var line = lines[row];
            for (var col = 0; col < line.Length; col++)
            {
                if (col == 0 || col == line.Length - 1 || 
                    row == 0 || row == lines.Length - 1)
                {
                    visibleCount++;
                    continue;
                }

                if (IsTreeVisible(row, col, lines))
                    visibleCount++;
            }
        }
        
        Console.WriteLine($"The number of trees visible from the outside is: {visibleCount}");
    }

    protected override void Gold(string[] lines)
    {
        var highestScore = 0;
        for (var row = 0; row < lines.Length; row++)
        {
            var line = lines[row];
            for (var col = 0; col < line.Length; col++)
            {
                var treeScenicScore = CalculateTreeScenicScore(row, col, lines);
                if (treeScenicScore > highestScore)
                    highestScore = treeScenicScore;
            }
        }

        Console.WriteLine($"The highest scenic score for any tree is: {highestScore}");
    }

    private bool IsTreeVisible(int row, int col, string[] lines)
    {
        var curLine = lines[row];
        var curTreeHeight = curLine[col];
        var visible = true;
        //left
        for (var c = col - 1; c >= 0; c--)
        {
            if (curLine[c] >= curTreeHeight)
            {
                visible = false;
                break;
            }
        }
        if (visible) return true;
        visible = true;
        
        //right
        for (var c = col + 1; c < curLine.Length; c++)
        {
            if (curLine[c] >= curTreeHeight)
            {
                visible = false;
                break;
            }
        }
        if (visible) return true;
        visible = true;
        
        //top
        for (var r = row - 1; r >= 0; r--)
        {
            if (lines[r][col] >= curTreeHeight)
            {
                visible = false;
                break;
            }
        }
        if (visible) return true;
        visible = true;
        
        //bottom
        for (var r = row + 1; r < lines.Length; r++)
        {
            if (lines[r][col] >= curTreeHeight)
            {
                visible = false;
                break;
            }
        }

        return visible;
    }
    
    private int CalculateTreeScenicScore(int row, int col, string[] lines)
    {
        var curLine = lines[row];
        var curTreeHeight = curLine[col];
        var leftScore = 0;
        var rightScore = 0;
        var topScore = 0;
        var bottomScore = 0;
        //left
        for (var c = col - 1; c >= 0; c--)
        {
            leftScore++;
            if (curLine[c] >= curTreeHeight)
            {
                break;
            }
        }
        
        //right
        for (var c = col + 1; c < curLine.Length; c++)
        {
            rightScore++;
            if (curLine[c] >= curTreeHeight)
            {
                break;
            }
        }
        
        //top
        for (var r = row - 1; r >= 0; r--)
        {
            topScore++;
            if (lines[r][col] >= curTreeHeight)
            {
                break;
            }
        }
        
        //bottom
        for (var r = row + 1; r < lines.Length; r++)
        {
            bottomScore++;
            if (lines[r][col] >= curTreeHeight)
            {
                break;
            }
        }

        return leftScore * rightScore * topScore * bottomScore;
    }
}