
int a = 0;
int i;
SomeMethod1(ref a);//语法错误
SomeMethod2(out i);//通过
void SomeMethod1(ref int i){
    int j = i;//通过
}
void SomeMethod2(out int i){
    i = 9;
}