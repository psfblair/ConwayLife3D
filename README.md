## Conway's Game of Life - 3D 

### A Unity Game with code in F\# 

This simulation is an extension of Conway's Game of Life from 2D to 3D.

From A. K. Dewdney's "Computer Recreations" column in Scientific American around 1987, which proposes a notation for 
rules of life-like simulations:

> Life wxyz is the rule set in which an alive cell will stay alive in the
> next generation if it has n live neighbors and w ≤ n ≤ x and a dead cell
> will become alive in the next generation if y ≤ n ≤ z.

For example, Rule 4555 means that a living cell dies if it has less than four or more than five 
living neighbors and a dead cell becomes alive if it has exactly five living neighbors.

For three dimensions, people seem to like 4555. See for instance, the comments in 
[life3d.c] (http://web.mit.edu/ghudson/dev/nokrb/third/afs-krb5/xlockmore/life3d.c):

```
/* 4555 life is probably the best */
/* 5766 life has gliders like 2d 2333 life */
/* There are no known gliders for 5655 or 6767 so the others may be better */
```

The current version of the game uses 4555. In the future I may make the rule configurable.