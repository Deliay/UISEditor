using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UISEditor.Data.Lexical;

namespace UISEditor.Data.Parser
{
    /*
     * uis -> cmds elements
     * 
     * *cmd -> @id string
     * *cmds -> cmds cmd | empty
     * 
     * *elements -> elements element
     * *element -> customEs | funcEs | aniEs
     * 
     * *customEs -> customEs customE | empty
     * *funcEs -> funcEs funcE | empty
     * *aniEs -> aniEs aniE | empty
     * 
     * *index -> number
     * *indexs -> indexs, index | empty
 
     * *customE -> _id \n props | _id-lists \n props
     * *funcE -> id \n props | id-lists \n props
     * *aniE -> :id \n aniCollects
     * *lists -> [indexs]
     * 
     * *prop -> tab id = value
     * *props -> props prop | empty
     * 
     * 
     * *aniProp -> aniName=value
     * *aniProps -> aniProps,aniProp
     * *aniCollects -> \n name=aniName \t \n aniCollects \n tab aniProps 
     * 
     * *animationRepeat -> number | r number | number, number | number, r number
     * *animationTime -> number | number, number | number, +number
     * *value -> term | string | vector | hexcolor | filename | framefile | motion
     * 
     * *vector -> term, term | (term, term)
     * *term -> expr - expr | expr + expr | expr
     * *expr -> number | px | percent
     * 
     * *motion -> !aniProps
     * *percent -> number%
     * *px -> number"px"
     * 
     * *filename -> string.string
     * *framefile -> string / number - number
     * */


    public class TokenReader
    {
        List<Token> tokens;
        int currentIndex = 0;
        public int RealLine { get; set; } = 1;
        public TokenReader(IEnumerable<Token> tokens)
        {
            this.tokens = new List<Token>(tokens);
        }

        public Token ReadLast()
        {
            return tokens[currentIndex - 2];
        }

        public Token Read()
        {
            var tok = tokens[currentIndex++];
            if (tok is EndOfLine) RealLine++;
            return tok;
        }

        public Token ReadNext()
        {
            return tokens[currentIndex];
        }

        public Token ReadRealNext()
        {
            return tokens[currentIndex + 1];
        }

        public Token ReadNext(int next)
        {
            return tokens[currentIndex + next];
        }

        public void Reset()
        {
            currentIndex = 0;
        }
    }

}
