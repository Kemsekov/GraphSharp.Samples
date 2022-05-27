using GraphSharp.GraphStructures;

IEnumerable<(int parentId, int[] children)> TestConnectionsList = 
new[]{
    (1,new[]{2,4,6,13}),
    (2,new[]{3,6,7,12}),
    (3,new[]{4,19}),
    (5,new[]{9,15,20}),
    (6,new[]{12,13,18}),
    (7,new[]{9,12,15}),
    (8,new[]{9,12,14,16}),
    (9,new[]{10}),
    (10,new[]{14,17,20}),
    (11,new[]{12,16,18}),
    (14,new[]{16}),
    (15,new[]{19}),
    (16,new[]{17})
};

//TODO: Implement the rest of the code
