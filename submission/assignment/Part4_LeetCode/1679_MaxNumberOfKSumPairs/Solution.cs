// LeetCode 1679. Max Number of K-Sum Pairs
// Approach: Two Pointers (sort, then move left/right toward the middle)
//   sum == k -> count the pair, move both pointers
//   sum <  k -> need a bigger sum  -> left++
//   sum >  k -> need a smaller sum -> right--
// Time: O(n log n) for sorting + O(n) scan. Space: O(1) extra.

using System;

public class Solution
{
    public int MaxOperations(int[] nums, int k)
    {
        Array.Sort(nums);

        int left = 0;
        int right = nums.Length - 1;
        int operations = 0;

        while (left < right)
        {
            long sum = (long)nums[left] + nums[right];

            if (sum == k)
            {
                operations++;
                left++;
                right--;
            }
            else if (sum < k)
            {
                left++;
            }
            else
            {
                right--;
            }
        }

        return operations;
    }
}