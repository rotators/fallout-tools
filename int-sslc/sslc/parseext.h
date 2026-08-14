#ifndef _PARSEEXT_H_
#define _PARSEEXT_H_

/*
	Extended SSL syntax for sfall
*/

void appendNodeListPart(NodeList* dst, const NodeList* src, int offset, int length);
void appendNodeList(NodeList* dst, const NodeList* src);

int parseArrayDereference(Procedure *p, NodeList *nodes, LexData symb, int *lastExprSize);
void parseArrayAssignment(Procedure *p, NodeList *nodes, LexData symb);
void parseFor(Procedure *p, NodeList *n);
void parseForEach(Procedure *p, NodeList *n);
void parseSwitch(Procedure *p, NodeList *n);
void parseAssocArrayExpression(Procedure *p, NodeList *n);
void parseArrayExpression(Procedure *p, NodeList *n);

#endif
